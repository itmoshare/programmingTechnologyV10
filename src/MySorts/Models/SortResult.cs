namespace MySorts.Models
{
    public class SortResult<TValue>
    {
        public SorterDescription<TValue> SorterDescription { get; set; }

        public TValue[] SortedArray { get; set; }

        public int ArrayLength { get; set; }

        public long TotalTimeMs { get; set; }
    }
}
