using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MovementsV2Demo
{
    static class Program
    {
        private static readonly IConfiguration Configuration = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json")
            .Build();

        private static readonly string Endpoint = Configuration["EndPointUrl"];
        private static readonly string DatabaseName = Configuration["DatabaseName"];
        private static readonly string Key = Configuration["AuthorizationKey"];

        public static async Task Main(string[] args)
        {
            var repository = new WarehouseRepository(DatabaseName, Endpoint, Key);

            bool exit = false;
            while (exit == false)
            {
                Console.Clear();
                Console.WriteLine($"Movements Modeling V2 Demo");
                Console.WriteLine($"-----------------------------------------");
                Console.WriteLine($"[a]   Create movements");
                Console.WriteLine($"[b]   Query Aggregate by articles");
                Console.WriteLine($"[c]   Query Aggregate by location");
                Console.WriteLine($"[x]   Exit");

                var result = Console.ReadKey(true);

                switch (result.KeyChar)
                {
                    case 'a':
                        Console.Clear();

                        Console.WriteLine("Enter amount of movements:");
                        var amount = Console.ReadLine();

                        await repository.CreateMovements(int.Parse(amount ?? string.Empty));
                        break;
                    case 'b':
                        Console.Clear();
                        await repository.GetAggregationByArticle();
                        break;
                    case 'c':
                        Console.Clear();
                        await repository.QueryAggregateByLocation();
                        break;
                    case 'x':
                        exit = true;
                        break;
                }
            }
        }
    }
}