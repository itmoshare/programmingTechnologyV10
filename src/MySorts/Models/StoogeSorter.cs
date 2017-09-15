using System;
using System.Collections.Generic;
using System.Linq;

namespace MySorts.Models
{
    public class StoogeSorter<TValue> : IArraySorter<TValue>
    {
        public TValue[] Sort(TValue[] source)
        {
            var res = source.ToArray();
            SortInner(res, 0, res.Length - 1);
            return res;
        }

        private static void SortInner(TValue[] array, int i, int j)
        {
            if (Comparer<TValue>.Default.Compare(array[i], array[j]) > 0)
            {
                var tmp = array[i];
                array[i] = array[j];
                array[j] = tmp;
            }
            if (j - i + 1 > 2)
            {
                int t = (int)Math.Floor((double)(j - i + 1) / 3);
                SortInner(array, i, j - t);
                SortInner(array, i + t, j);
                SortInner(array, i, j - t);
            }
        }
    }
}
