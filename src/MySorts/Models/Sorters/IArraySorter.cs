namespace MySorts.Models.Sorters
{
    public interface IArraySorter<TValue>
    {
        TValue[] Sort(TValue[] source);
    }
}
