using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspBlueprinter.Diagnosis
{
    using System;
    using System.Collections.Generic;

#if DEBUG && false
    public class DbgDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TKey: notnull
    {
        public delegate void DiagnosticHandler(string message);
        public event DiagnosticHandler? OnDiagnostic = Console.WriteLine;

        public new void Add(TKey key, TValue value)
        {
            OnDiagnostic?.Invoke($"Adding key: {key}, value: {value}");
            base.Add(key, value);
        }

        public new TValue this[TKey key]
        {
            get
            {
                OnDiagnostic?.Invoke($"Getting key: {key}, value: {base[key]}");

                return base[key];
            }
            set
            {
                OnDiagnostic?.Invoke($"Setting key: {key}, value: {value}");
                base[key] = value;
            }
        }

        public DbgDictionary() : base() { }

        public DbgDictionary(int capacity) : base(capacity) { }

        public DbgDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }

        public DbgDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }

        public DbgDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }

        public DbgDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer) { }

        protected virtual void OnDiagnosticMessage(string message)
        {
            OnDiagnostic?.Invoke(message);
        }
    }
#endif
}
