using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PLGL
{
    internal static class Extensions
    {
        public static void RemoveFirst<T>(this List<T> list, Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException("Predicate parameter cannot be null.");

            for (int i = 0; i < list.Count; i++)
            {
                if (match(list[i]) == true)
                {
                    list.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
