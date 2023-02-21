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

using System.Runtime.InteropServices;
using Zmod4410evz.Sensor;

namespace Zmod4410evz.Interop
{
    /// <summary>
    /// Device structure ZMOD4xxx
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Zmod4xxxDevice
    {
        public Zmod4xxxDevice(byte address, Zmod4xxxI2c read, Zmod4xxxI2c write, Zmod4xxxDelay delay)
        {
            Configuration = new byte[Zmod44xxConstants.ConfigurationLength];
            Delay = delay;
            I2cAddr = address;
            InitConfiguration = Zmod4xxxConfiguration.CreateInitConfiguration();
            MeasurementConfiguration = Zmod4xxxConfiguration.CreateMeasurementConfiguration();
            MoxEr = 0;
            MoxLr = 0;
            Pid = Zmod44xxConstants.Pid;
            ProductionData = new byte[Zmod44xxConstants.ProductionDataLength];
            I2cRead = read;
            I2cWrite = write;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="RegAddress"></param>
        /// <param name="DataBuffer"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public delegate byte Zmod4xxxI2c(
            byte Address,
            Byte RegAddress,
            [Out, In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] byte[] DataBuffer,
            Byte Length);

        /// <summary>
        ///
        /// </summary>
        /// <param name="ms"></param>
        public delegate void Zmod4xxxDelay(UInt32 ms);

        /// <summary>
        /// i2c address of the sensor
        /// </summary>
        public byte I2cAddr;

        /// <summary>
        /// Configuration parameter set
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Zmod44xxConstants.ConfigurationLength)]
        public byte[] Configuration;

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
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Zmod44xxConstants.ProductionDataLength)]
        public byte[] ProductionData;

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
        public Zmod4xxxConfiguration InitConfiguration;

        /// <summary>
        /// The measurement configuration
        /// </summary>
        public Zmod4xxxConfiguration MeasurementConfiguration;

        internal void CalculateFactorMeasurementConfig(byte[] hsp)
        {
            CalculateFactor(MeasurementConfiguration, hsp);
        }

        internal void CalculateFactorInitConfig(byte[] hsp)
        {
            CalculateFactor(InitConfiguration, hsp);
        }

        private void CalculateFactor(Zmod4xxxConfiguration config, byte[] hsp)
        {
            short[] hsp_temp = new short[Zmod44xxConstants.HspLength];
            float hspf;

            for (int i = 0; i < config.h.Length; i += 2)
            {
                hsp_temp[i / 2] = ((short)
                    ((config.h.Buffer[i] << 8) + config.h.Buffer[i + 1]));
                hspf = (-((float)Configuration[2] * 256.0F + Configuration[3]) *
                        ((Configuration[4] + 640.0F) * (Configuration[5] + hsp_temp[i / 2]) -
                         512000.0F)) / 12288000.0F;

                hsp[i] = (byte)((ushort)hspf >> 8);
                hsp[i + 1] = (byte)((ushort)hspf & 0x00FF);
            }
        }
    }
}