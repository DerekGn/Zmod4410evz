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

namespace Zmod4410evz.Interop
{
    public static class Iaq
    {
        internal static IaqError Calc(
            ref IaqHandle handle,
            ref Zmod4xxxDevice device,
            ref IaqInputs input,
            ref IaqResults results)
        {
            return calc_iaq_2nd_gen(ref handle, ref device, ref input, ref results);
        }

        internal static IaqError Init(ref IaqHandle handle)
        {
            return init_iaq_2nd_gen(ref handle);
        }

        [DllImport("lib_iaq_2nd_gen")]
        extern static IaqError calc_iaq_2nd_gen(
            ref IaqHandle handle,
            ref Zmod4xxxDevice device,
            ref IaqInputs input,
            ref IaqResults results);

        [DllImport("lib_iaq_2nd_gen")]
        extern static IaqError init_iaq_2nd_gen(ref IaqHandle handle);
    }
}
