// /*
//   SharpNative - C# to D Transpiler
//   (C) 2014 Irio Systems 
// */

#region Imports

using System.Collections.Concurrent;
using System.Collections.Generic;

#endregion

namespace SharpNative.Compiler
{
    internal sealed class ConcurrentHashSet<T>
    {
        private readonly ConcurrentDictionary<T, byte> _data = new ConcurrentDictionary<T, byte>();

        public int Count
        {
            get { return _data.Count; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _data.Keys.GetEnumerator();
        }

        public bool Remove(T t)
        {
            byte b;
            return _data.TryRemove(t, out b);
        }

        public bool Add(T t)
        {
            return _data.TryAdd(t, 0);
        }

        public bool IsEmpty()
        {
            return Count == 0;
        }

        public bool Contains(T t)
        {
            return _data.ContainsKey(t);
        }
    }
}