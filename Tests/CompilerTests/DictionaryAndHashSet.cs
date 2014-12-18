using System;
using System.Linq;
using System.Collections.Generic;

namespace Blargh
{
    public static class Utilities
    {
        public static void SomeFunction()
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            dict.Add(4, 3);
            Console.WriteLine(dict[4]);
            Console.WriteLine(dict.ContainsKey(8));
            dict.Remove(4);
            foreach(int key in dict.Keys)
                Console.WriteLine(key);
            foreach(int val in dict.Values)
                Console.WriteLine(val);
            foreach(var kv in dict)
                Console.WriteLine(kv.Key + " " + kv.Value);
            var dict2 = dict.ToDictionary(o => o.Key, o => o.Value);
            var vals = dict.Values;
            
            HashSet<int> hash = new HashSet<int>();
            hash.Add(999);
            Console.WriteLine(hash.Contains(999));
            hash.Remove(999);
            Console.WriteLine(hash.Contains(999));
            foreach(int hashItem in hash)
                Console.WriteLine(hashItem);
            var z = hash.Select(o => 3).ToArray();
            var g = hash.GroupBy(o => o).Select(o => o.Count()).Min();
        }
    }
}