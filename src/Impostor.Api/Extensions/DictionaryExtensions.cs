using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Impostor.Api
{
    public static class DictionaryExtensions
    {
        /// <summary>Returns a read-only <see cref="ReadOnlyDictionary{TKey,TValue}" /> wrapper for the dictionary.</summary>
        /// <param name="dictionary">An <see cref="IDictionary{TKey,TValue}" /> to create a <see cref="ReadOnlyDictionary{TKey,TValue}" /> from.</param>
        /// <typeparam name="TKey">The type of the keys of <paramref name="dictionary" />.</typeparam>
        /// <typeparam name="TValue">The type of the values of <paramref name="dictionary" />.</typeparam>
        /// <returns>An object that acts as a read-only wrapper around the <see cref="IDictionary{TKey,TValue}" />.</returns>
        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
            where TKey : notnull
        {
            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }
    }
}
