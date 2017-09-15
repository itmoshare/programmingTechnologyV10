using System;
using System.IO;
using System.Linq;

namespace MySorts.Models
{
    public static class ArrayReader<TValue>
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
                .Select(x => (TValue) Convert.ChangeType(x, typeof(TValue)))
                .ToArray();
        }
    }
}
