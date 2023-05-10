﻿using Domain.Entities;
using Domain.Queries.GetReceipts;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace Data.Queries.PipelineStages
{
    internal static class FilterReceiptItemsStage
    {
        internal static PipelineDefinition<Receipt, BsonDocument> FilterReceiptItems(
            this PipelineDefinition<Receipt, BsonDocument> pipelineDefinition,
            GetReceiptsFilter queryFilter)
        {
            var matchFilter = BuildMatchFilter(queryFilter);

            if (matchFilter != FilterDefinition<BsonDocument>.Empty)
            {
                pipelineDefinition = pipelineDefinition.Match(matchFilter);
            }

            return pipelineDefinition;
        }

        private static FilterDefinition<BsonDocument> BuildMatchFilter(GetReceiptsFilter queryFilter)
        {
            var filters = new List<FilterDefinition<BsonDocument>>
            {
                MatchByItemNames(queryFilter),
            };

            filters.RemoveAll(x => x == FilterDefinition<BsonDocument>.Empty);

            if (!filters.Any())
            {
                return FilterDefinition<BsonDocument>.Empty;
            }

            return filters.Count == 1 ? filters.First() : Builders<BsonDocument>.Filter.And(filters);
        }

        private static FilterDefinition<BsonDocument> MatchByItemNames(
            GetReceiptsFilter queryFilter)
        {
            if (!queryFilter.ItemNames.Any())
            {
                return FilterDefinition<BsonDocument>.Empty;
            }

            var itemNames = queryFilter.ItemNames
                .Select(x => new BsonRegularExpression(new Regex(x, RegexOptions.IgnoreCase)));

            var filter = new BsonDocument(
                "ReceiptItems.ItemName",
                new BsonDocument("$in", BsonArray.Create(itemNames)));

            return new BsonDocumentFilterDefinition<BsonDocument>(filter);
        }
    }
}