using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MySorts.Models.Sorters
{
    public class MultipleSorter<TValue>
    {
        private readonly List<SorterDescription<TValue>> _sorterDescriptions;

        public MultipleSorter(IEnumerable<SorterDescription<TValue>> sorterDescriptions)
        {
            _sorterDescriptions = new List<SorterDescription<TValue>>(sorterDescriptions);
        }

        public async Task<IEnumerable<SortResult<TValue>>> Sort(TValue[] array)
        {
            List<SortResult<TValue>> res = new List<SortResult<TValue>>();
            await Task.Run(() =>
            {
                foreach (var sorterDescription in _sorterDescriptions)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    sorterDescription.Sorter.Sort(array);
                    stopwatch.Stop();
                    res.Add(new SortResult<TValue>
                    {
                        ArrayLength = array.Length,
                        SorterDescription = sorterDescription,
                        TotalTimeMs = stopwatch.ElapsedMilliseconds
                    });
                }
            });
            return res;
        }
    }
}
