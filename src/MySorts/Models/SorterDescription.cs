using MySorts.Models.Sorters;

namespace MySorts.Models
{
    public class SorterDescription<TValue>
    {
        public string SortAlgName { get; }

        public IArraySorter<TValue> Sorter { get; }

        public SorterDescription(string sortAlgName, IArraySorter<TValue> sorter)
        {
            SortAlgName = sortAlgName;
            Sorter = sorter;
        }
    }
}
