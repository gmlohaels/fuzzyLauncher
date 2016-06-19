using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Helpers
{
    public static class LinqHelper
    {
        public static IEnumerable<TValue> IgnoreExceptions<TValue>(this IEnumerable<TValue> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            using (var e = source.GetEnumerator())
            {
                while (true)
                {
                    TValue value;
                    try
                    {
                        if (!e.MoveNext())
                            yield break;
                        value = e.Current;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        continue;
                    }
                    yield return value;
                }
            }
        }
    }
}
