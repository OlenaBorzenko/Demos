using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MovementsV1Demo
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
                Console.WriteLine($"Movements Modeling V1 Demo");
                Console.WriteLine($"-----------------------------------------");
                Console.WriteLine($"[a]   Create movements");
                Console.WriteLine($"[b]   Create stock checkpoint");
                Console.WriteLine($"[c]   Query Aggregate by articles");
                Console.WriteLine($"[d]   Query Aggregate by location");
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
                        await repository.CreateAggregationByLocation();
                        break;
                    case 'c':
                        Console.Clear();
                        await repository.GetAggregationByArticle();
                        break;
                    case 'd':
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