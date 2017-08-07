using Miktemk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BookAligner.Code
{
    public class AlgorithmArrayAlign
    {
        /// <summary>
        /// NOTE: this functions assumes pos1 and pos2 are ordered in asc order
        /// </summary>
        public ArrayAlignIndexPair[] GetBestAlignment(int[] pos1, int[] pos2, int thresh)
        {
            var result = new List<ArrayAlignIndexPair>();

            int i1 = 0, i2 = 0;
            while (i1 < pos1.Length && i2 < pos2.Length)
            {
                if (Math.Abs(pos1[i1] - pos2[i2]) <= thresh)
                {
                    result.Add(new ArrayAlignIndexPair(i1, i2));
                    i1++;
                    i2++;
                }
                else if (pos1[i1] < pos2[i2])
                    i1++;
                else //if (pos1[i1] > pos2[i2]) // NOTE: theoretically this can be uncommented, but just in case I dont want to get stuck
                    i2++;
            }

            return result.ToArray();
        }
    }

    public class ArrayAlignIndexPair
    {
        public ArrayAlignIndexPair(int i1, int i2)
        {
            Index1 = i1;
            Index2 = i2;
        }

        public int Index1 { get; set; }
        public int Index2 { get; set; }
    }
}
