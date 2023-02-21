/*
* MIT License
*
* Copyright (c) 2023 Derek Goslin https://github.com/DerekGn
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/

using HidSharp;
using McMaster.Extensions.CommandLineUtils;
using MCP2221IO.Usb;
using MCP2221IO;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Zmod4410evz.Sensor;

namespace Zmod4410evz.Commands
{
    internal abstract class BaseCommand
    {
        private readonly IServiceProvider _serviceProvider;

        protected BaseCommand(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            Serial = String.Empty;
        }

        [Option(Templates.Vid, "The VID of the MCP2221", CommandOptionType.SingleValue)]
        public int Vid { get; set; } = 0x04D8;

        [Option(Templates.Pid, "The PID of the MCP2221", CommandOptionType.SingleValue)]
        public int Pid { get; set; } = 0x00DD;

        [Option(Templates.SerialNumber, "The MCP2221 instance serial number", CommandOptionType.SingleValue)]
        public string Serial { get; set; }

        [Required]
        [Range(typeof(byte), "0x07", "0x78")]
        [Option(Templates.I2cAddress, "The I2C device address", CommandOptionType.SingleValue)]
        public byte Address { get; set; } = Zmod44xxConstants.Address;

        protected int ExecuteCommand(Func<IZmod4410, int> action)
        {
            int result = -1;

            try
            {
                var hidDevice = DeviceList.Local.GetHidDeviceOrNull(Vid, Pid, null, Serial);

                if (hidDevice != null)
                {
                    using HidSharpHidDevice hidSharpHidDevice = new((ILogger<IHidDevice>)_serviceProvider.GetService(typeof(ILogger<IHidDevice>)), hidDevice);
                    using MCP2221IO.Device device = new((ILogger<IDevice>)_serviceProvider.GetService(typeof(ILogger<IDevice>)), hidSharpHidDevice);

                    device.Open();
                    
                    device.SetI2cBusSpeed(100000);

                    using IZmod4410 sensor = new Zmod4410((ILogger<IZmod4410>)_serviceProvider.GetService(typeof(ILogger<IZmod4410>)), device, Address);

                    result = action(sensor);
                }
                else
                {
                    Console.Error.WriteLine($"Unable to find HID device VID: [0x{Vid:X}] PID: [0x{Vid:X}] SerialNumber: [{Serial}]");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An unhandled exception occurred: {ex}");
            }

            return result;
        }

        protected virtual int OnExecute(CommandLineApplication app, IConsole console)
        {
            console.Error.WriteLine("You must specify a command. See --help for more details.");
            app.ShowHelp();

            return 0;
        }
    }
}
