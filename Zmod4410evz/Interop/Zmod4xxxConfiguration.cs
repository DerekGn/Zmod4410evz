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
    /// Structure to hold the gas sensor module configuration.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Zmod4xxxConfiguration
    {
        public byte Start;
        public Zmod4xxxConfigurationString h;
        public Zmod4xxxConfigurationString d;
        public Zmod4xxxConfigurationString m;
        public Zmod4xxxConfigurationString s;
        public Zmod4xxxConfigurationString r;
        public byte ProdDataLength;

        public static Zmod4xxxConfiguration CreateInitConfiguration()
        {
            return new Zmod4xxxConfiguration()
            {
                Start = 0x80,
                h = new Zmod4xxxConfigurationString(Zmod44xxConstants.HAddress, new byte[] { 0x00, 0x50 }),
                d = new Zmod4xxxConfigurationString(Zmod44xxConstants.DAddress, new byte[] { 0x00, 0x28 }),
                m = new Zmod4xxxConfigurationString(Zmod44xxConstants.MAddress, new byte[] { 0xC3, 0xE3 }),
                s = new Zmod4xxxConfigurationString(Zmod44xxConstants.SAddress, new byte[] { 0x00, 0x00, 0x80, 0x40 }),
                r = new Zmod4xxxConfigurationString(Zmod44xxConstants.RAddress, new byte[4])
            };
        }

        public static Zmod4xxxConfiguration CreateMeasurementConfiguration()
        {
            return new Zmod4xxxConfiguration()
            {
                Start = 0x80,
                h = new Zmod4xxxConfigurationString(Zmod44xxConstants.HAddress, 
                new byte[]
                {
                    0x00, 0x50, 0xFF, 0x38,
                    0xFE, 0xD4, 0xFE, 0x70,
                    0xFE, 0x0C, 0xFD, 0xA8,
                    0xFD, 0x44, 0xFC, 0xE0 
                }),
                d = new Zmod4xxxConfigurationString(Zmod44xxConstants.DAddress,
                new byte[] 
                {
                    0x00, 0x52, 0x02, 0x67,
                    0x00, 0xCD, 0x03, 0x34
                }),
                m = new Zmod4xxxConfigurationString(Zmod44xxConstants.MAddress, 
                new byte[] 
                { 
                    0x23, 0x03, 0xA3, 0x43 
                }),
                s = new Zmod4xxxConfigurationString(Zmod44xxConstants.SAddress, 
                new byte[] 
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
                r = new Zmod4xxxConfigurationString(Zmod44xxConstants.RAddress, new byte[32]),
                ProdDataLength = Zmod44xxConstants.ProductionDataLength
            };
        }
    }
}
