using System.Collections.Generic;
using System.Linq;

namespace QuestPDF.Elements.Table
{
    /// <summary>
    /// This dictionary allows to access key that does not exist.
    /// Instead of throwing an exception, it returns a default value.
    /// </summary>
    internal class DynamicDictionary<TKey, TValue>
    {
        private TValue Default { get; }
        private IDictionary<TKey, TValue> Dictionary { get; } = new Dictionary<TKey, TValue>();

        public DynamicDictionary()
        {
            
        }
        
        public DynamicDictionary(TValue defaultValue)
        {
            Default = defaultValue;
        }
        
        public TValue this[TKey key]
        {
            get => Dictionary.TryGetValue(key, out var value) ? value : Default;
            set => Dictionary[key] = value;
        }

        public List<KeyValuePair<TKey, TValue>> Items => Dictionary.ToList();
    }
}