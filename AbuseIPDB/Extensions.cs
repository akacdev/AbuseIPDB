using System;
using System.Collections.Generic;
using System.Linq;

namespace AbuseIPDB
{
    public static class Extensions
    {
        /// <summary>
        /// A small helper extension method to get all flags of an enum using LINQ.<br/>
        /// Source: <a href="https://stackoverflow.com/a/66275102/10974450">https://stackoverflow.com/a/66275102/10974450</a>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetFlags<T>(this T instance) where T : struct, Enum
        {
            return Enum.GetValues<T>().Where(member => instance.HasFlag(member));
        }
    }
}