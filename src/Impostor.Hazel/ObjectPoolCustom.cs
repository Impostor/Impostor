using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Impostor.Hazel
{
    /// <summary>
    ///     A fairly simple object pool for items that will be created a lot.
    /// </summary>
    /// <typeparam name="T">The type that is pooled.</typeparam>
    /// <threadsafety static="true" instance="true"/>
    public sealed class ObjectPoolCustom<T> where T : IRecyclable
    {
        private int numberCreated;
        public int NumberCreated { get { return numberCreated; } }

        public int NumberInUse { get { return this.inuse.Count; } }
        public int NumberNotInUse { get { return this.pool.Count; } }
        public int Size { get { return this.NumberInUse + this.NumberNotInUse; } }

#if HAZEL_BAG
        private readonly ConcurrentBag<T> pool = new ConcurrentBag<T>();
#else
        private readonly List<T> pool = new List<T>();
#endif

        // Unavailable objects
        private readonly ConcurrentDictionary<T, bool> inuse = new ConcurrentDictionary<T, bool>();

        /// <summary>
        ///     The generator for creating new objects.
        /// </summary>
        /// <returns></returns>
        private readonly Func<T> objectFactory;
        
        /// <summary>
        ///     Internal constructor for our ObjectPool.
        /// </summary>
        internal ObjectPoolCustom(Func<T> objectFactory)
        {
            this.objectFactory = objectFactory;
        }

        /// <summary>
        ///     Returns a pooled object of type T, if none are available another is created.
        /// </summary>
        /// <returns>An instance of T.</returns>
        internal T GetObject()
        {
#if HAZEL_BAG
            if (!pool.TryTake(out T item))
            {
                Interlocked.Increment(ref numberCreated);
                item = objectFactory.Invoke();
            }
#else
            T item;
            lock (this.pool)
            {
                if (this.pool.Count > 0)
                {
                    var idx = this.pool.Count - 1;
                    item = this.pool[idx];
                    this.pool.RemoveAt(idx);
                }
                else
                {
                    Interlocked.Increment(ref numberCreated);
                    item = objectFactory.Invoke();
                }
            }
#endif

            if (!inuse.TryAdd(item, true))
            {
                throw new Exception("Duplicate pull " + typeof(T).Name);
            }

            return item;
        }

        /// <summary>
        ///     Returns an object to the pool.
        /// </summary>
        /// <param name="item">The item to return.</param>
        internal void PutObject(T item)
        {
            if (inuse.TryRemove(item, out bool b))
            {
#if HAZEL_BAG
                pool.Add(item);
#else
                lock (this.pool)
                {
                    pool.Add(item);
                }
#endif
            }
            else
            {
#if DEBUG
                throw new Exception("Duplicate add " + typeof(T).Name);
#endif
            }
        }
    }
}
