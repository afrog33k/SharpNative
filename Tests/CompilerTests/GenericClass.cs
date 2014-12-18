using System;
using System.Collections.Generic;

namespace Blargh
{
    public class KeyValueList<K, V> : IEquatable<K>
    {
        private List<KeyValuePair<K, V>> _list = new List<KeyValuePair<K, V>>();

        public void Add(K key, V value)
        {
            this._list.Add(new KeyValuePair<K, V>(key, value));
        }

        public void Insert(int index, K key, V value)
        {
            _list.Insert(index, new KeyValuePair<K, V>(key, value));
        }

        public void Clear()
        {
            _list.Clear();
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public bool Equals(K other)
        {
            throw new NotImplementedException();
        }
    }
}