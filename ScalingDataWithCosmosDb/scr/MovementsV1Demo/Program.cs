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

        private static readonly string Connection = Configuration["CosmosDBConnection"];
        private static readonly string DatabaseName = Configuration["DatabaseName"];

        public static async Task Main(string[] args)
        {
            var repository = new WarehouseRepository(DatabaseName, Connection);

            bool exit = false;
            while (exit == false)
            {
                Console.Clear();
                Console.WriteLine($"Movements Modeling V1 Demo");
                Console.WriteLine($"-----------------------------------------");
                Console.WriteLine($"[a]   Create movements");
                Console.WriteLine($"[b]   Create stock checkpoint");
                Console.WriteLine($"[c]   Get stock by location");
                Console.WriteLine($"[d]   Get stock by articles");
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
                        await repository.CreateStockCheckpointByLocation();
                        break;
                    case 'c':
                        Console.Clear();
                        await repository.GetStockByLocations();
                        break;
                    case 'd':
                        Console.Clear();
                        await repository.GetStockByArticles();
                        break;
                    case 'x':
                        exit = true;
                        break;
                }
            }
        }
    }
}