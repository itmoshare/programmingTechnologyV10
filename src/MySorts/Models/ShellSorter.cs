using System;
using System.Collections.Generic;
using System.Linq;

namespace MySorts.Models
{
    class ShellSorter<TValue> : IArraySorter<TValue>
    {
        public TValue[] Sort(TValue[] source)
        {
            var res = source.ToArray();
            ShellSort(res);
            return res;
        }

        static void ShellSort(TValue[] arr)
        {
            int i, j, inc;
            inc = 3;
            while (inc > 0)
            {
                for (i = 0; i < arr.Length; i++)
                {
                    j = i;
                    var temp = arr[i];
                    while ((j >= inc) && (Comparer<TValue>.Default.Compare(arr[j - inc], temp) > 0))
                    {
                        arr[j] = arr[j - inc];
                        j = j - inc;
                    }
                    arr[j] = temp;
                }
                if (inc / 2 != 0)
                    inc = inc / 2;
                else if (inc == 1)
                    inc = 0;
                else
                    inc = 1;
            }
        }
    }
}
