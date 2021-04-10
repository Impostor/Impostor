using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Impostor.Api
{
    public static class DictionaryExtensions
    {
        /// <summary>Returns a read-only <see cref="T:System.Collections.ObjectModel.ReadOnlyDictionary`2" /> wrapper for the dictionary.</summary>
        /// <returns>An object that acts as a read-only wrapper around the <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
            where TKey : notnull
        {
            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }
    }
}
