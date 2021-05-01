using System;
using System.Collections.Generic;

namespace Shared
{
    public static class Helpers
    {
        private static readonly Random Random = new Random();

        public static T GetItemByRandomIndex<T>(List<T> items)
        {
            int index = Random.Next(0, items.Count);
            var result = items[index];

            return result;
        }
    }
}