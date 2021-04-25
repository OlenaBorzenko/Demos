using System;
using Shared;

namespace MovementsV2Demo
{
    public static class ModelsHelper
    {
        public static ArticleMovement CreateArticleMovementModel()
        {
            // Getting random test data;
            var article = Helpers.GetItemByRandomIndex(TestData.Articles);
            var movement = Helpers.GetItemByRandomIndex(TestData.Movements);

            return new ArticleMovement
            {
                Id = Guid.NewGuid().ToString(),
                ArticleId = article.Id,
                ArticleName = article.Name,
                MovementType = movement.Type,
                FromLocationId = movement.From,
                ToLocationId = movement.To,
                TimeStamp = DateTimeOffset.Now
            };
        }
    }
}