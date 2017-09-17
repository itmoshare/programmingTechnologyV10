using System.Collections.Generic;
using System.Linq;

namespace MySorts.Models
{
    public class QuickSorter<TValue> : IArraySorter<TValue>
    {
        public TValue[] Sort(TValue[] source)
        {
            var res = source.ToArray();
            QuicksortInner(res, 0, res.Length - 1);
            return res;
        }

        private static void QuicksortInner(TValue[] elements, int left, int right)
        {
            int i = left, j = right;
            var pivot = elements[(left + right) / 2];

            while (i <= j)
            {
                while (Comparer<TValue>.Default.Compare(elements[i], pivot) < 0)
                {
                    i++;
                }

                while (Comparer<TValue>.Default.Compare(elements[j], pivot) > 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    var tmp = elements[i];
                    elements[i] = elements[j];
                    elements[j] = tmp;

                    i++;
                    j--;
                }
            }

            if (left < j)
            {
                QuicksortInner(elements, left, j);
            }

            if (i < right)
            {
                QuicksortInner(elements, i, right);
            }
        }
    }
}
