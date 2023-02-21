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

namespace Zmod4410evz
{
    /// <summary>
    /// Variables that receive the algorithm outputs.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct IaqResults
    {
        /// <summary>
        /// MOx resistance.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 13)]
        public float[] Rmox;
        /// <summary>
        /// Log10 of CDA resistance.
        /// </summary>
        public float LogRcda;
        /// <summary>
        /// Heater resistance.
        /// </summary>
        public float Rhtr;
        /// <summary>
        /// Ambient temperature (degC).
        /// </summary>
        public float Temperature;
        /// <summary>
        /// IAQ index.
        /// </summary>
        public float Iaq;
        /// <summary>
        /// TVOC concentration (mg/m^3).
        /// </summary>
        public float Tvoc;
        /// <summary>
        /// EtOH concentration (ppm).
        /// </summary>
        public float Etoh;
        /// <summary>
        /// eCO2 concentration (ppm).
        /// </summary>
        public float Eco2;
    }
}