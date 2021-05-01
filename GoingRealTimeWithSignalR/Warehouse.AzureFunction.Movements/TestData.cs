using System.Collections.Generic;
using Shared;

namespace WarehouseAzureFunctionMovements
{
     public static class TestData
        {
            public static readonly List<ArticleItem> Articles = new List<ArticleItem>
            {
                new ArticleItem
                {
                    Name = "Sprite",
                    Id = "ed9c74f0-7cfe-4faa-b584-37c0dde43ddf"
                },
                new ArticleItem
                {
                    Name = "Fanta",
                    Id = "f5d4738c-c66e-45f3-ba10-ffb12e04e9fc"
                },
                new ArticleItem
                {
                    Name = "Cola",
                    Id = "a7843f7a-6797-496a-83ab-6d4de1d86318"
                },
                new ArticleItem
                {
                    Name = "Water",
                    Id = "7a1deba0-6488-40f4-9a20-40bd0270697d"
                },
                new ArticleItem
                {
                    Name = "Tonic",
                    Id = "044dc200-a1b0-42f6-9ba9-1de2b6116a43"
                },
                new ArticleItem
                {
                    Name = "Beer",
                    Id = "925d7807-bf68-47cb-a6b7-623d341fb0a8"
                },
            };

            public static readonly List<MovementItem> Movements = new List<MovementItem>
            {
                new MovementItem
                {
                    From = "vehicle",
                    To = "1-1-1",
                    Type = "inbound"
                },
                new MovementItem
                {
                    From = "1-1-1",
                    To = "2-2-2",
                    Type = "refilling"
                },
                new MovementItem
                {
                    From = "2-2-2",
                    To = "3-3-3",
                    Type = "picking"
                },
                new MovementItem
                {
                    From = "3-3-3",
                    To = "4-4-4",
                    Type = "handover"
                },
                new MovementItem
                {
                    From = "4-4-4",
                    To = "5-5-5",
                    Type = "customer"
                }

            };
        }

}