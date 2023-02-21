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

namespace Zmod4410evz.Sensor
{
    internal static class Zmod44xxConstants
    {
        public const int AdcLength = 32;
        public const byte Address = 0x32;
        public const int ConfigurationLength = 6;
        public const byte DAddress = 0x50;
        public const byte HAddress = 0x40;
        public const int HspLength = 8;
        public const byte MAddress = 0x60;
        public const ushort Pid = 0x2310;
        public const int ProductionDataLength = 7;
        public const byte RAddress = 0x97;
        public const int ResultMax = 32;
        public const byte SAddress = 0x68;
        public const int TrackingNumberLength = 6;
    }
}
