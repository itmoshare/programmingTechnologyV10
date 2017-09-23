using System.Collections.Generic;

namespace MySorts.Models.DemoSorter
{
    public interface IDemoSorter<TValue>
    {
        string Name { get; }

        IEnumerable<DemoSortStep<TValue>> Steps(IEnumerable<TValue> arr);
    }
}
