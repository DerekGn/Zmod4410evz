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
using Zmod4410evz.Sensor.Exceptions;

namespace Zmod4410evz.Sensor
{
    internal class Zmod4410 : IZmod4410
    {
        private readonly byte _address;
        private readonly List<byte> _configuration;
        private readonly ILogger<IZmod4410> _logger;
        private readonly List<byte> _productionData;
        private readonly Zmod4410Configuration _initConfiguration;
        private readonly Zmod4410Configuration _measurementConfiguration;
        private IDevice _device;
        private ushort _moxEr = 0;
        private ushort _moxLr = 0;

        public Zmod4410(ILogger<IZmod4410> logger, IDevice device, byte address)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _device = device ?? throw new ArgumentNullException(nameof(device));

            _measurementConfiguration =
                new Zmod4410Configuration(
                0x80,
                Zmod44xxConstants.ProductionDataLength,
                new Zmod4410ConfigurationString(Zmod44xxConstants.HAddress,
                new List<byte>()
                {
                    0x00, 0x50, 0xFF, 0x38,
                    0xFE, 0xD4, 0xFE, 0x70,
                    0xFE, 0x0C, 0xFD, 0xA8,
                    0xFD, 0x44, 0xFC, 0xE0
                }),
                new Zmod4410ConfigurationString(Zmod44xxConstants.DAddress,
                new List<byte>()
                {
                    0x00, 0x52, 0x02, 0x67,
                    0x00, 0xCD, 0x03, 0x34
                }),
                new Zmod4410ConfigurationString(Zmod44xxConstants.MAddress,
                new List<byte>()
                {
                    0x23, 0x03, 0xA3, 0x43
                }),
                new Zmod4410ConfigurationString(Zmod44xxConstants.SAddress,
                new List<byte>()
                {
                    0x00, 0x00, 0x06, 0x49,
                    0x06, 0x4A, 0x06, 0x4B,
                    0x06, 0x4C, 0x06, 0x4D,
                    0x06, 0x4E, 0x06, 0x97,
                    0x06, 0xD7, 0x06, 0x57,
                    0x06, 0x4E, 0x06, 0x4D,
                    0x06, 0x4C, 0x06, 0x4B,
                    0x06, 0x4A, 0x86, 0x59
                }),
                new Zmod4410ConfigurationString(Zmod44xxConstants.RAddress, (new byte[32]).ToList()));

            _initConfiguration = new Zmod4410Configuration(
                0x80,
                0,
                new Zmod4410ConfigurationString(Zmod44xxConstants.HAddress, new List<byte>() { 0x00, 0x50 }),
                new Zmod4410ConfigurationString(Zmod44xxConstants.DAddress, new List<byte>() { 0x00, 0x28 }),
                new Zmod4410ConfigurationString(Zmod44xxConstants.MAddress, new List<byte>() { 0xC3, 0xE3 }),
                new Zmod4410ConfigurationString(Zmod44xxConstants.SAddress, new List<byte>() { 0x00, 0x00, 0x80, 0x40 }),
                new Zmod4410ConfigurationString(Zmod44xxConstants.RAddress, (new byte[4]).ToList()));

            _configuration = new List<byte> { };
            _productionData = new List<byte> { };

            _address = address;
        }

        public IReadOnlyList<byte> Configuration => _configuration;

        public byte I2cAddress => _address;

        public Zmod4410Configuration InitConfiguration => _initConfiguration;

        public Zmod4410Configuration MeasurementConfiguration => _measurementConfiguration;

        public ushort MoxEr => _moxEr;

        public ushort MoxLr => _moxLr;

        public ushort Pid => Zmod44xxConstants.Pid;

        public IReadOnlyList<byte> ProductionData => _productionData;
        public ErrorEvent GetErrorEvent()
        {
            var buffer = new byte[1];

            I2cRead(_address, 0xB7, buffer, 1);

            return (ErrorEvent)buffer[0];
        }

        public void GetInformation()
        {
            byte status = 0;
            ushort count = 0;

            do
            {
                I2cWrite(_address, Zmod44xxI2cRegisters.AddressCommand, new byte[] { 0 }, 1);

                status = GetStatus();

                Delay(200);

                count++;
            } while (((status & 0x80) != 0x00) && (count < 1000));

            if (count >= 1000)
            {
                throw new Zmod4410Exception(Zmod4xxxError.ErrorGasTimeout, "Gas Timeout");
            }

            var buffer = new byte[sizeof(ushort)];

            I2cRead(_address, Zmod44xxI2cRegisters.AddressPid, buffer, sizeof(ushort));

            ushort pid = (ushort)((buffer[0] * 256) + buffer[1]);

            if (Zmod44xxConstants.Pid != pid)
            {
                throw new Zmod4410Exception(
                    Zmod4xxxError.ErrorSensorUnsupported,
                    $"Sensor unsupported Expected Pid: 0x{Zmod44xxConstants.Pid:X} Actual: 0x{pid:X}");
            }

            buffer = new byte[Zmod44xxConstants.ConfigurationLength];

            I2cRead(
                _address,
                Zmod44xxI2cRegisters.AddressConf,
                buffer,
                Zmod44xxConstants.ConfigurationLength);

            _configuration.Clear();
            _configuration.AddRange(buffer);

            buffer = new byte[Zmod44xxConstants.ProductionDataLength];

            I2cRead(
                _address,
                Zmod44xxI2cRegisters.AddressProdData,
                buffer,
                Zmod44xxConstants.ProductionDataLength);

            _productionData.Clear();
            _productionData.AddRange(buffer);
        }

        public byte GetStatus()
        {
            byte[] buffer = new byte[1];

            I2cRead(_address, Zmod44xxI2cRegisters.AddressStatus, buffer, 1);

            return buffer[0];
        }

        public IReadOnlyList<byte> GetTrackingNumber()
        {
            var buffer = new byte[Zmod44xxConstants.TrackingNumberLength];

            I2cRead(_address, Zmod44xxI2cRegisters.AddressTracking, buffer, Zmod44xxConstants.TrackingNumberLength);

            return buffer;
        }

        public void PrepareSensor()
        {
            InitSensor();

            Delay(50);

            InitMeasurement();
        }

        public IReadOnlyList<byte> ReadAdc()
        {
            var buffer = new byte[MeasurementConfiguration.R.Length];

            I2cRead(
                _address,
                _measurementConfiguration.R.Address,
                buffer,
                _measurementConfiguration.R.Length);

            return buffer;
        }

        public void StartMeasurement()
        {
            I2cWrite(
                _address,
                Zmod44xxI2cRegisters.AddressCommand,
                new byte[] { _measurementConfiguration.Start }, 1);
        }
        internal void CalculateFactorInitConfig(byte[] hsp)
        {
            CalculateFactor(_initConfiguration, hsp);
        }

        internal void CalculateFactorMeasurementConfig(byte[] hsp)
        {
            CalculateFactor(_measurementConfiguration, hsp);
        }

        private void CalculateFactor(Zmod4410Configuration config, byte[] hsp)
        {
            short[] hsp_temp = new short[Zmod44xxConstants.HspLength];
            float hspf;

            for (int i = 0; i < config.H.Length; i += 2)
            {
                hsp_temp[i / 2] = ((short)
                    ((config.H.Buffer[i] << 8) + config.H.Buffer[i + 1]));
                hspf = (-((float)Configuration[2] * 256.0F + Configuration[3]) *
                        ((Configuration[4] + 640.0F) * (Configuration[5] + hsp_temp[i / 2]) -
                         512000.0F)) / 12288000.0F;

                hsp[i] = (byte)((ushort)hspf >> 8);
                hsp[i + 1] = (byte)((ushort)hspf & 0x00FF);
            }
        }

        private void Delay(uint delay)
        {
            _logger.LogDebug("Executing Delay: {Delay}", delay);
            Thread.Sleep((int)delay);
        }

        private byte I2cRead(byte address, byte regAddress, byte[] buffer, byte count)
        {
            _logger.LogDebug("Executing Read Address: [0x{Address:X}] RegAddress: [0x{RegAddress:X}] Length: [0x{Count:X}]", address, regAddress, count);
            var i2cAddress = new I2cAddress(address);

            _device.I2cWriteDataNoStop(i2cAddress, new List<byte>() { regAddress });

            _device.I2cReadDataRepeatedStart(i2cAddress, count).CopyTo(buffer, 0);

            return 0;
        }

        private byte I2cWrite(byte address, byte regAddress, byte[] buffer, byte count)
        {
            _logger.LogDebug("Executing Write Address: [0x{Address:X}] RegAddress: [0x{RegAddress:X}] Length: [0x{Count:X}]", address, regAddress, count);

            var payload = new List<byte>
            {
                regAddress
            };

            payload.AddRange(buffer.Take(count));

            _device.I2cWriteData(new I2cAddress(address), payload);

            return 0;
        }

        private void InitMeasurement()
        {
            var hsp = new byte[Zmod44xxConstants.HspLength * 2];

            CalculateFactorMeasurementConfig(hsp);

            I2cWrite(
                _address,
                _measurementConfiguration.H.Address,
                hsp,
                _measurementConfiguration.H.Length);

            WriteConfiguration(_measurementConfiguration);
        }

        private void InitSensor()
        {
            var buffer = new byte[Zmod44xxConstants.ResultMax];
            var hsp = new byte[Zmod44xxConstants.HspLength * 2];

            I2cRead(_address, 0xB7, buffer, 1);

            CalculateFactorInitConfig(hsp);

            I2cWrite(
                _address,
                _initConfiguration.H.Address,
                hsp,
                _initConfiguration.H.Length);

            WriteConfiguration(InitConfiguration);

            I2cWrite(
                _address,
                Zmod44xxI2cRegisters.AddressCommand,
                new byte[] { _initConfiguration.Start },
                1);

            byte status;

            do
            {
                status = GetStatus();

                Delay(50);
            } while ((status & Zmod4410Status.SequencerRunningMask) == Zmod4410Status.SequencerRunningMask);


            I2cRead(
                _address,
                _initConfiguration.R.Address,
                buffer,
                _initConfiguration.R.Length);

            _moxLr = (ushort)((buffer[0] << 8) | buffer[1]);
            _moxEr = (ushort)((buffer[2] << 8) | buffer[3]);
        }
        private void WriteConfiguration(Zmod4410Configuration configuration)
        {
            I2cWrite(
                _address,
                configuration.D.Address,
                configuration.D.Buffer.ToArray(),
                configuration.D.Length);

            I2cWrite(
                _address,
                configuration.M.Address,
                configuration.M.Buffer.ToArray(),
                configuration.M.Length);

            I2cWrite(
                _address,
                configuration.S.Address,
                configuration.S.Buffer.ToArray(),
                configuration.S.Length);
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