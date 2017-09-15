namespace MySorts.Models
{
    public interface IArraySorter<TValue>
    {
        TValue[] Sort(TValue[] source);
    }
}
