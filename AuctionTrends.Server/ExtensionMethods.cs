using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;

namespace AuctionTrends.Server
{
    public static class ExtensionMethods
    {
        public static List<string> ToList(this StringCollection collection)
        {
            var list = collection.Cast<string>().ToList();
            return list;
        }

        public static void AddRange(this ItemCollection collection, List<string> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        public static void RemoveAll(this ItemCollection collection, IList items)
        {
            foreach (var item in items)
            {
                collection.Remove(item);
            }
        }

        public static void RemoveAll(this StringCollection collection, IList items)
        {
            foreach (var item in items)
            {
                collection.Remove((string)item);
            }
        }

        public static IList Clone(this IList listToClone)
        {
            return listToClone.Cast<object>().ToList();
        }
    }
}