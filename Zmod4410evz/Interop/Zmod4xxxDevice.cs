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

using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices;

namespace Zmod4410evz.Interop
{
    /// <summary>
    /// Device structure ZMOD4xxx
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Zmod4xxxDevice : IDisposable
    {
        private GCHandle _gchConfiguration;
        private GCHandle _gchProductionData;
        private GCHandle _gchInitConfiguration;
        private GCHandle _gchMeasurementConfiguration;

        public Zmod4xxxDevice(
            byte i2cAddress,
            byte[] configuration,
            ushort moxEr,
            ushort moxLr,
            ushort pid,
            byte[] productionData,
            Zmod4xxxI2c i2cRead,
            Zmod4xxxI2c i2cWrite,
            Zmod4xxxDelay delay,
            Zmod4xxxConfiguration initConfiguration,
            Zmod4xxxConfiguration measurementConfiguration)
        {
            I2cAddr = i2cAddress;

            _gchConfiguration = GCHandle.Alloc(configuration);
            Configuration = GCHandle.ToIntPtr(_gchConfiguration);

            MoxEr = moxEr;
            MoxLr = moxLr;
            Pid = pid;

            _gchProductionData = GCHandle.Alloc(productionData);
            ProductionData = GCHandle.ToIntPtr(_gchProductionData);

            I2cRead = i2cRead;
            I2cWrite = i2cWrite;
            Delay = delay;

            _gchInitConfiguration = GCHandle.Alloc(initConfiguration);
            InitConfiguration = GCHandle.ToIntPtr(_gchProductionData);

            _gchMeasurementConfiguration = GCHandle.Alloc(measurementConfiguration);
            MeasurementConfiguration = GCHandle.ToIntPtr(_gchMeasurementConfiguration);
        }

        public delegate byte Zmod4xxxI2c(
            byte Address,
            Byte RegAddress,
            [Out, In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] byte[] DataBuffer,
            Byte Length);

        public delegate void Zmod4xxxDelay(UInt32 ms);

        /// <summary>
        /// i2c address of the sensor
        /// </summary>
        public byte I2cAddr;

        /// <summary>
        /// Configuration parameter set
        /// </summary>
        IntPtr Configuration;

        /// <summary>
        /// Sensor specific parameter
        /// </summary>
        public ushort MoxEr;

        /// <summary>
        /// Sensor specific parameter
        /// </summary>
        public ushort MoxLr;

        /// <summary>
        /// Product id of the sensor
        /// </summary>
        public ushort Pid;

        /// <summary>
        /// Production data
        /// </summary>
        public IntPtr ProductionData;

        /// <summary>
        /// i2c read function callback
        /// </summary>
        public Zmod4xxxI2c I2cRead;

        /// <summary>
        /// i2c write function callback
        /// </summary>
        public Zmod4xxxI2c I2cWrite;

        /// <summary>
        /// The delay function callback
        /// </summary>
        public Zmod4xxxDelay Delay;

        /// <summary>
        /// The init configuration
        /// </summary>
        public IntPtr InitConfiguration;

        /// <summary>
        /// The measurement configuration
        /// </summary>
        public IntPtr MeasurementConfiguration;

        public void Dispose()
        {
            if (Configuration != IntPtr.Zero)
            {
                _gchConfiguration.Free();
                Configuration = IntPtr.Zero;
            }

            if (ProductionData != IntPtr.Zero)
            {
                _gchProductionData.Free();
                ProductionData = IntPtr.Zero;
            }
        }
    }
}