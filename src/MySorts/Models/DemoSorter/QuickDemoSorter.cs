using System.Collections.Generic;
using System.Linq;

namespace MySorts.Models.DemoSorter
{
    public class QuickDemoSorter<TValue> : IDemoSorter<TValue>
    {
        public string Name => "Quick sort";

        public IEnumerable<DemoSortStep<TValue>> Steps(IEnumerable<TValue> arr)
        {
            var res = arr.ToArray();
            return QuicksortInner(res, 0, res.Length - 1);
        }

        private static IEnumerable<DemoSortStep<TValue>> QuicksortInner(TValue[] elements, int left, int right)
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

                    yield return new DemoSortStep<TValue>()
                    {
                        NewArr = elements.ToArray(),
                        SwappedIndex1 = i,
                        SwappedIndex2 = j
                    };

                    i++;
                    j--;
                }
            }

            if (left < j)
            {
                foreach (var demoSortStep in QuicksortInner(elements, left, j))
                {
                    yield return demoSortStep;
                }
            }

            if (i < right)
            {
                foreach (var demoSortStep in QuicksortInner(elements, i, right))
                {
                    yield return demoSortStep;
                }
            }
        }
    }
}
