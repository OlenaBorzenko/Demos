using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;

namespace Shared
{
    public static class Helpers
    {
        private static readonly Random Random = new();

        public static void Print(object obj)
        {
            Console.WriteLine($"{JObject.FromObject(obj)}\n");
        }

        public static T GetItemByRandomIndex<T>(List<T> items)
        {
            int index = Random.Next(0, items.Count);
            var result = items[index];

            return result;
        }

        public static async Task MakeRequestWithStopWatch<T>(Func<Task<ItemResponse<T>>> action, string methodName)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var response = await action();

            stopWatch.Stop();

            Console.WriteLine($"{methodName}: Request Charge: {response.RequestCharge}. Time spend in ms: {stopWatch.Elapsed.Milliseconds}");
        }


        public static async Task<FeedResponse<T>> MakeRequestWithStopWatch<T>(Func<Task<FeedResponse<T>>> action, string methodName)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var response = await action();

            stopWatch.Stop();

            if (response.Count == 0)
            {
                return null;
            }

            Console.WriteLine($"\n{methodName}: Request Charge: {response.RequestCharge}. Time spend in ms: {stopWatch.Elapsed.Milliseconds}\n");

            return response;
        }
    }
}