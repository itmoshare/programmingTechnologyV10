using System.Collections.Generic;
using System.Linq;

namespace MySorts.Models.Sorters
{
    public class BubleSorter<TValue> : IArraySorter<TValue>
    {
        public TValue[] Sort(TValue[] source)
        {
            TValue[] res = source.ToArray();
            for (int write = 0; write < res.Length; write++)
            {
                for (int sort = 0; sort < res.Length - 1; sort++)
                {
                    if (Comparer<TValue>.Default.Compare(res[sort], res[sort + 1]) > 0)
                    {
                        var temp = res[sort + 1];
                        res[sort + 1] = res[sort];
                        res[sort] = temp;
                    }
                }
            }
            return res;
        }
    }
}
