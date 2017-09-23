namespace MySorts.Models.DemoSorter
{
    public class DemoSortStep<TValue>
    {
        public TValue[] NewArr { get; set; }

        public int SwappedIndex1 { get; set; }

        public int SwappedIndex2 { get; set; }
    }
}
