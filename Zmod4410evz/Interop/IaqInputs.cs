﻿/*
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
    [StructLayout(LayoutKind.Sequential)]
    internal struct IaqInputs : IDisposable
    {
        private GCHandle _gch;

        /// <summary>
        /// Sensor raw values
        /// </summary>
        public IntPtr AdcResult;
        /// <summary>
        /// Relative Humditiy in percentage
        /// </summary>
        public float HumidityPct;
        /// <summary>
        /// Ambient Temperature in C
        /// </summary>
        public float TemperatureDegc;

        public void AssignAdcResult(byte[] adcResult)
        {
            ReleaseAdcResult();

            _gch = GCHandle.Alloc(adcResult);

            AdcResult = GCHandle.ToIntPtr(_gch);
        }

        public void Dispose()
        {
            ReleaseAdcResult();
        }

        private void ReleaseAdcResult()
        {
            if (AdcResult != IntPtr.Zero)
            {
                _gch.Free();
                AdcResult = IntPtr.Zero;
            }
        }
    }
}