using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing
{
    internal class CircularBuffer
    {
        private const int BufferSize = 11_000;

        private object[] Buffer = new object[BufferSize];
        private int WriteIndex { get; set; } = 0;
        private int ReadIndex { get; set; } = 0;
        
        public T Get<T>() where T : class, new()
        {
            lock (this)
            {
                if (ReadIndex == WriteIndex)
                    return new T();
                
                var index = ReadIndex;
                ReadIndex = (ReadIndex + 1) % BufferSize;
                
                var result = Buffer[index] as T;
                Buffer[index] = null;
                
                return result;
            }
        }

        public void Store(object value)
        {
            lock (this)
            {
                Buffer[WriteIndex] = value;
                WriteIndex = (WriteIndex + 1) % BufferSize;
            }
        }
    }
    
    // performance analysis:
    // without: 115s
    // ConcurrentQueue: 28s
    // ConcurrentBag: 38s
    // CircularBuffer: 30s
    internal static class ElementCacheManager
    {
        private static ConcurrentDictionary<Type, ConcurrentQueue<object>> Cache { get; } = new();

        public static T Get<T>() where T : class, new()
        {
            var buffer = Cache.GetOrAdd(typeof(T), _=> new ConcurrentQueue<object>());
            return buffer.TryDequeue(out var result) ? result as T : new T();
        }

        public static void Store<T>(T element)
        {
            var buffer = Cache.GetOrAdd(element.GetType(), _=> new ConcurrentQueue<object>());
            buffer.Enqueue(element);
        }

        public static void Collect(Element element)
        {
            foreach (var child in element.GetChildren())
                Collect(child);

            if (element is ICollectable collectable)
            {
                collectable.Collect();
                Store(element);   
            }
        }
    }
}