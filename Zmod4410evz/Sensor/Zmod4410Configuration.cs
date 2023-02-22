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
    internal class Zmod4410Configuration
    {
        public Zmod4410Configuration(
            byte start,
            byte productDataLength,
            Zmod4410ConfigurationString h,
            Zmod4410ConfigurationString d,
            Zmod4410ConfigurationString m,
            Zmod4410ConfigurationString s,
            Zmod4410ConfigurationString r)
        {
            Start = start;
            ProductDataLength = productDataLength;
            H = h;
            D = d;
            M = m;
            S = s;
            R = r;
        }

        public Zmod4410ConfigurationString D { get; }
        public Zmod4410ConfigurationString H { get; }
        public Zmod4410ConfigurationString M { get; }
        public byte ProductDataLength;
        public Zmod4410ConfigurationString R { get; }
        public Zmod4410ConfigurationString S { get; }
        public byte Start;
    }
}
