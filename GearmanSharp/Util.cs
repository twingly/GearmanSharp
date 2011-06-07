using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Twingly.Gearman
{
    public static class Util
    {


        /// <summary>
        /// Splits a byte array on \0. Works as String.Split
        /// </summary>
        public static byte[][] SplitArray(byte[] arr)
        {
            const byte splitByte = 0;

            var segments = new List<byte[]>();
            int lastPos = 0;
            while (true)
            {
                var pos = Array.IndexOf(arr, splitByte, lastPos);
                if (pos == -1)
                {
                    pos = arr.Length;
                }

                var len = pos - lastPos;
                var segment = new byte[len];
                Array.Copy(arr, lastPos, segment, 0, len);
                segments.Add(segment);

                if (pos < arr.Length)
                    lastPos = pos + 1; // account for the byte we split on
                else
                    break;
            }

            return segments.ToArray();
        }
    }
}
