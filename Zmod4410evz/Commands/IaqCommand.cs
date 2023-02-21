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

using McMaster.Extensions.CommandLineUtils;
using Zmod4410evz.Interop;

namespace Zmod4410evz.Commands
{
    [Command(Description = "Execute Iaq sampling")]
    [Subcommand(typeof(IaqUlpCommand))]
    internal class IaqCommand : BaseCommand
    {
        public IaqCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override int OnExecute(CommandLineApplication app, IConsole console)
        {
            return ExecuteCommand((sensor) =>
            {
                console.WriteLine("Reading Sensor Information");

                sensor.GetInformation();

                console.WriteLine("Reading Sensor Tracking Information");

                var trackingNumber = sensor.GetTrackingNumber();

                console.WriteLine($"Sensor tracking number: [{Convert.ToHexString(trackingNumber.ToArray())}]");

                var trimingData = sensor.GetTrimingData();

                console.WriteLine($"Sensor trimming data: [{Convert.ToHexString(trimingData.ToArray())}]");

                Console.WriteLine("Preparing Sensor");

                sensor.PrepareSensor();

                return 0;
            });
        }
    }
}
