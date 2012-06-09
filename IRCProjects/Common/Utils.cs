using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jiyuu.Common
{
    public static class Utils
    {
        private static readonly sbyte[] _lookup = new sbyte[] { 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46 };
        public static unsafe string ToHex(this byte[] arr)
        {
            int len = arr.Length;
            int i = 0;

            sbyte* chars = stackalloc sbyte[len * 2];
            fixed (byte* pSrc = arr)
            {
                byte* pIn = pSrc;
                sbyte* pOut = chars;
                while (i++ < len)
                {
                    *pOut++ = _lookup[*pIn >> 4];
                    *pOut++ = _lookup[*pIn++ & 0xF];
                }
            }
            return new String(chars, 0, len * 2);
        }
    }
}
