using System.Collections.Generic;
using System.Linq;

namespace MySorts.Models.DemoSorter
{
    public class ShellDemoSorter<TValue> : IDemoSorter<TValue>
    {
        public string Name => "Сортировка Шелла";

        public IEnumerable<DemoSortStep<TValue>> Steps(IEnumerable<TValue> source)
        {
            var arr = source.ToArray();
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
                    yield return new DemoSortStep<TValue>
                    {
                        NewArr = arr.ToArray(),
                        SwappedIndex1 = i,
                        SwappedIndex2 = j
                    };
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
