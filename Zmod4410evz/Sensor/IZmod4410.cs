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

using Zmod4410evz.Interop;

namespace Zmod4410evz.Sensor
{
    internal interface IZmod4410 : IDisposable
    {
        /// <summary>
        /// Configuration parameter set
        /// </summary>
        IReadOnlyList<byte> Configuration { get; }

        /// <summary>
        /// The I2C Address of the device
        /// </summary>
        byte I2cAddress { get; }

        /// <summary>
        /// The init configuration
        /// </summary>
        Zmod4410Configuration InitConfiguration { get; }

        /// <summary>
        /// The measurement configuration
        /// </summary>
        Zmod4410Configuration MeasurementConfiguration { get; }

        /// <summary>
        /// Sensor specific parameter
        /// </summary>
        ushort MoxEr { get;  }

        /// <summary>
        /// Sensor specific parameter
        /// </summary>
        ushort MoxLr { get; }

        /// <summary>
        /// Product id of the sensor
        /// </summary>
        ushort Pid { get; }

        IReadOnlyList<byte> ProductionData { get; }

        float CalculateRmox();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ErrorEvent GetErrorEvent();

        /// <summary>
        /// Read the sensor information
        /// </summary>
        /// <remarks></remarks>
        void GetInformation();

        /// <summary>
        /// Read the sensor status
        /// </summary>
        /// <returns></returns>
        byte GetStatus();

        /// <summary>
        /// Read the tracking number
        /// </summary>
        /// <returns>The tracking number bytes</returns>
        IReadOnlyList<byte> GetTrackingNumber();

        //void ExecuteCleaning();

        /// <summary>
        /// Prepare the sensor calibration factors
        /// </summary>
        void PrepareSensor();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<byte> ReadAdc();

        /// <summary>
        /// Start a measurement
        /// </summary>
        void StartMeasurement();

        Zmod4xxxDevice ToMarshalType();
    }
}