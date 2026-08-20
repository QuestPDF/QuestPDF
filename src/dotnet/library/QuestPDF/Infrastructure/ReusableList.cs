using System;
using System.Collections.Generic;

namespace QuestPDF.Infrastructure
{
    /// <summary>
    /// A pooled list used by layout elements to build rendering commands without allocating on the hot path.
    /// Rent with <see cref="ReusableListPool{T}.Get"/>, ideally inside a using statement,
    /// so the buffer returns to the pool on every exit path.
    /// </summary>
    internal sealed class ReusableList<T> : List<T>, IDisposable
    {
        internal bool IsRented;

        public void Dispose()
        {
            if (!IsRented)
                return;

            IsRented = false;
            ReusableListPool<T>.Return(this);
        }
    }

    /// <summary>
    /// Thread-local pool of <see cref="ReusableList{T}"/> instances.
    /// A rented list is always returned on the thread that rented it (rendering is single-threaded
    /// per document), so no cross-thread synchronization is needed.
    /// </summary>
    internal static class ReusableListPool<T>
    {
        // do not pool oversized buffers, so one huge element cannot
        // keep a large array alive for the lifetime of the thread
        private const int MaxPooledCapacity = 4096;

        [ThreadStatic]
        private static Stack<ReusableList<T>>? Pool;

        public static ReusableList<T> Get()
        {
            var pool = Pool;

            var list = pool == null || pool.Count == 0
                ? new ReusableList<T>()
                : pool.Pop();

            list.IsRented = true;
            return list;
        }

        public static void Return(ReusableList<T> list)
        {
            if (list.Capacity > MaxPooledCapacity)
                return;

            list.Clear();
            (Pool ??= new Stack<ReusableList<T>>()).Push(list);
        }
    }
}
