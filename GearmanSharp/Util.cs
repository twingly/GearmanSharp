using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Twingly.Gearman
{
    public static class Util
    {
        /// <summary>
        /// Concatenates a number of byte arrays with \0 between them.
        /// </summary>
        public static byte[] JoinByteArraysForData(params byte[][] data)
        {
            int len = (data.Length == 0 ? 0 : data.Length - 1);
            foreach (var arr in data)
            {
                len += arr.Length;
            }

            var result = new byte[len];
            var offset = 0;
            bool first = true;
            foreach (var arr in data)
            {
                // Add \0 before all values, except for the first. (i.e. append it for all but the last)
                if (first)
                    first = false;
                else
                    result[offset++] = 0;
                Array.Copy(arr, 0, result, offset, arr.Length);
                offset += arr.Length;
            }

            return result;
        }
    }
}
