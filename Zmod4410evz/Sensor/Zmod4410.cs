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

using MCP2221IO;
using Microsoft.Extensions.Logging;
using Zmod4410evz.Interop;
using Zmod4410evz.Sensor.Exceptions;

namespace Zmod4410evz.Sensor
{
    internal class Zmod4410 : IZmod4410
    {
        private readonly ILogger<IZmod4410> _logger;
        private IDevice _device;
        private Zmod4xxxDevice _zmod4XxxDevice;
        public Zmod4410(ILogger<IZmod4410> logger, IDevice device, byte address)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _device = device ?? throw new ArgumentNullException(nameof(device));

            _zmod4XxxDevice =
                    new(address,
                        new Zmod4xxxDevice.Zmod4xxxI2c(Read),
                        new Zmod4xxxDevice.Zmod4xxxI2c(Write),
                        new Zmod4xxxDevice.Zmod4xxxDelay(Delay));
        }

        public void GetInformation()
        {
            byte status = 0;
            ushort count = 0;

            do
            {
                _zmod4XxxDevice.I2cWrite(_zmod4XxxDevice.I2cAddr, Zmod44xxI2cRegisters.AddressCommand, new byte[] { 0 }, 1);

                status = GetStatus();

                _zmod4XxxDevice.Delay(200);

                count++;
            } while (((status & 0x80) != 0x00) && (count < 1000));

            if (count >= 1000)
            {
                throw new Zmod4410Exception(Zmod4xxxError.ErrorGasTimeout, "Gas Timeout");
            }

            var buffer = new byte[sizeof(ushort)];

            _zmod4XxxDevice.I2cRead(_zmod4XxxDevice.I2cAddr, Zmod44xxI2cRegisters.AddressPid, buffer, sizeof(ushort));

            ushort pid = (ushort)((buffer[0] * 256) + buffer[1]);

            if (_zmod4XxxDevice.Pid != pid)
            {
                throw new Zmod4410Exception(
                    Zmod4xxxError.ErrorSensorUnsupported,
                    $"Sensor unsupported Expected Pid: 0x{_zmod4XxxDevice.Pid:X} Actual: 0x{pid:X}");
            }

            _zmod4XxxDevice.I2cRead(
                _zmod4XxxDevice.I2cAddr,
                Zmod44xxI2cRegisters.AddressConf,
                _zmod4XxxDevice.Configuration,
                Zmod44xxConstants.ConfigurationLength);

            _zmod4XxxDevice.I2cRead(
                _zmod4XxxDevice.I2cAddr,
                Zmod44xxI2cRegisters.AddressProdData,
                _zmod4XxxDevice.ProductionData,
                Zmod44xxConstants.ProductionDataLength);
        }

        public byte GetStatus()
        {
            byte[] buffer = new byte[1];

            _zmod4XxxDevice.I2cRead(_zmod4XxxDevice.I2cAddr, Zmod44xxI2cRegisters.AddressStatus, buffer, 1);

            return buffer[0];
        }

        public IReadOnlyList<byte> GetTrackingNumber()
        {
            var buffer = new byte[Zmod44xxConstants.TrackingNumberLength];

            _zmod4XxxDevice.I2cRead(_zmod4XxxDevice.I2cAddr, Zmod44xxI2cRegisters.AddressTracking, buffer, Zmod44xxConstants.TrackingNumberLength);

            return buffer;
        }

        public IReadOnlyList<byte> GetTrimingData()
        {
            return _zmod4XxxDevice.ProductionData;
        }

        public void PrepareSensor()
        {
            InitSensor();

            _zmod4XxxDevice.Delay(50);

            InitMeasurement();
        }

        private void Delay(uint delay)
        {
            _logger.LogDebug("Executing Delay: {Delay}", delay);
            Thread.Sleep((int)delay);
        }

        private void InitMeasurement()
        {
            throw new NotImplementedException();
        }

        private void InitSensor()
        {
            var buffer = new byte[32];
            var hsp = new byte[Zmod44xxConstants.HspLength * 2];

            _zmod4XxxDevice.I2cRead(_zmod4XxxDevice.I2cAddr, 0xB7, buffer, 1);
        }

        private byte Read(byte address, byte regAddress, byte[] buffer, byte count)
        {
            _logger.LogDebug("Executing Read Address: [0x{Address:X}] RegAddress: [0x{RegAddress:X}] Length: [0x{Count:X}]", address, regAddress, count);
            var i2cAddress = new I2cAddress(address);

            _device.I2cWriteDataNoStop(i2cAddress, new List<byte>() { regAddress });

            _device.I2cReadDataRepeatedStart(i2cAddress, count).CopyTo(buffer, 0);

            return 0;
        }

        private byte Write(byte address, byte regAddress, byte[] buffer, byte count)
        {
            _logger.LogDebug("Executing Write Address: [0x{Address:X}] RegAddress: [0x{RegAddress:X}] Length: [0x{Count:X}]", address, regAddress, count);

            var payload = new List<byte>
            {
                regAddress
            };
            payload.AddRange(buffer);

            _device.I2cWriteData(new I2cAddress(address), payload);

            return 0;
        }
        #region Dispose

        private bool disposedValue;

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _logger.LogDebug($"Disposing {nameof(Zmod4410)}");

            if (!disposedValue)
            {
                if (disposing)
                {
                    _device = null;
                }

                disposedValue = true;
            }
        }
        #endregion Dispose
    }
}