using System;
using System.Collections.Generic;
using System.Threading;

namespace QuestPDF.Infrastructure
{
    /// <summary>
    /// A pooled list used by layout elements to build rendering commands without allocating on the hot path.
    /// Rent with <see cref="Get"/>, ideally inside a using statement,
    /// so the buffer returns to the pool on every exit path.
    /// </summary>
    /// <remarks>
    /// Pools are thread-local, so renting and returning never contends with other threads;
    /// an instance disposed on a different thread than it was rented on simply joins that thread's pool.
    /// A rented instance is still a plain <see cref="List{T}"/> and must not be mutated concurrently.
    /// </remarks>
    internal sealed class ReusableList<T> : List<T>, IDisposable
    {
        // do not pool oversized buffers, so one huge element cannot
        // keep a large array alive for the lifetime of the thread
        private const int MaxPooledCapacity = 4096;

        [ThreadStatic]
        private static Stack<ReusableList<T>>? Pool;

        // 1 = rented, 0 = available; int so Interlocked can guard against double-dispose
        private int IsRented;

        public static ReusableList<T> Get()
        {
            var pool = Pool;

            var list = pool is { Count: > 0 }
                ? pool.Pop()
                : new ReusableList<T>();

            list.IsRented = 1;
            return list;
        }

        public void Dispose()
        {
            // without this guard, disposing twice (possibly from two threads) would pool
            // the same instance twice, and two later Get calls would share one buffer
            if (Interlocked.Exchange(ref IsRented, 0) == 0)
                return;

            if (Capacity > MaxPooledCapacity)
                return;

            Clear();
            (Pool ??= new Stack<ReusableList<T>>()).Push(this);
        }
    }
}
