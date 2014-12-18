using System;
using System.Collections.Generic;

public static class Program
{
    private static class Funcs<T> {
        public static readonly Func<T, bool> True = x => true;
        public static readonly Func<T, T> Identity = x => x;
    }

    public static IEnumerable<T> Where<T>
    (this IEnumerable<T> source, Func<T, bool> predicate) {
        //Check(source, predicate);
        return WhereIterator(source, predicate);
    }

    private static IEnumerable<T> WhereIterator<T>
    (IEnumerable<T> source, Func<T, bool> predicate) {
        List<T> results = new List<T>();
        foreach (T item in source) {
            if (predicate(item)) {
                results.Add(item); 
            }
        }

        return results;
    }


    public static IEnumerable<TResult> Select<T, TResult>
    (this IEnumerable<T> source, Func<T, TResult> selector) {
        //Check(source, selector);
        return SelectIterator(source, selector);
    }

    private static IEnumerable<TResult> SelectIterator<T, TResult>
    (IEnumerable<T> source, Func<T, TResult> selector) {
        List<TResult> results = new List<TResult>();
        foreach (T item in source) {
            results.Add(selector(item));
        }
        return results;
    }


    public class Boy
    {
        public string Age;
        public override string ToString ()
        {
            return string.Format ("[Boy]: Age = {0}", Age);
        }
    }

    public static void Main()
    {
        var list  = new int [] {1,2,3,4};

        var listGreaterThan2 = list.Where (k => k > 2).Select(j=>(j+5.6).ToString());//.Select(j=>new Boy(){Age=j.ToString()});

        foreach(var item in listGreaterThan2)
        {
            Console.WriteLine(item);
        }


        var list2  = new string [] {"boy","ball","hello","absolute"};

        var listStartsWithB = list2.Where(k=>k.StartsWith("b"));

        foreach(var item in listStartsWithB)
        {
            Console.WriteLine(item);
        }

    }
}