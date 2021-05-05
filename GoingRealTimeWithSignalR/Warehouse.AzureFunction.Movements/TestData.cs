using System.Collections.Generic;
using WarehouseAzureFunctionMovements.Models;

namespace WarehouseAzureFunctionMovements
{
     public static class TestData
        {
            public static readonly List<ArticleItem> Articles = new List<ArticleItem>
            {
                new ArticleItem
                {
                    name = "Sprite",
                    id = "ed9c74f0-7cfe-4faa-b584-37c0dde43ddf"
                },
                new ArticleItem
                {
                    name = "Fanta",
                    id = "f5d4738c-c66e-45f3-ba10-ffb12e04e9fc"
                },
                new ArticleItem
                {
                    name = "Cola",
                    id = "a7843f7a-6797-496a-83ab-6d4de1d86318"
                },
                new ArticleItem
                {
                    name = "Water",
                    id = "7a1deba0-6488-40f4-9a20-40bd0270697d"
                },
                new ArticleItem
                {
                    name = "Tonic",
                    id = "044dc200-a1b0-42f6-9ba9-1de2b6116a43"
                },
                new ArticleItem
                {
                    name = "Beer",
                    id = "925d7807-bf68-47cb-a6b7-623d341fb0a8"
                },
            };

            public static readonly List<MovementItem> Movements = new List<MovementItem>
            {
                new MovementItem
                {
                    from = "1-1-1",
                    to = "2-2-2",
                    type = "refilling"
                },
                new MovementItem
                {
                    from = "2-2-2",
                    to = "3-3-3",
                    type = "picking"
                },
            };
        }

}