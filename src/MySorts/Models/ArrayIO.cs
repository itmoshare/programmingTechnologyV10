using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MySorts.Models
{
    // ReSharper disable once InconsistentNaming
    public static class ArrayIO<TValue>
    {
        public static TValue[] Read(string str)
        {
            using (var s = new StringReader(str))
            {
                return Read(s);
            }
        }

        public static TValue[] Read(TextReader stream)
        {
            var str = stream.ReadToEnd();
            return str
                .Split(' ')
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => (TValue) Convert.ChangeType(x, typeof(TValue)))
                .ToArray();
        }

        public static async Task Write(TextWriter writer, TValue[] arr)
        {
            var res = string.Concat(arr.Select(x => x.ToString() + " "));
            await writer.WriteLineAsync(res);
        }
    }
}
