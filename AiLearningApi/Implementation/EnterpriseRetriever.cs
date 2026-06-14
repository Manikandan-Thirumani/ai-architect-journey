using AiLearningApi.Models;
using AiLearningApi.Services;
using AiLearningApi.Services.Retrieval;

namespace AiLearningApi.Implementation
{
    public class EnterpriseRetriever : IEnterpriseRetriever
    {
        private readonly ILlmQueryExpander _queryExpander;
        private readonly IMultiQueryRetriever _multiRetriever;
        private readonly IRrfRanker _rrfRanker;
        private readonly IKeywordSearchService _keywordSearch;
        private readonly HybridRanker _hybridRanker;
        private readonly ContextOptimizer _contextOptimizer;

        public EnterpriseRetriever(
            ILlmQueryExpander queryExpander,
            IMultiQueryRetriever multiRetriever,
            IRrfRanker rrfRanker,
            IKeywordSearchService keywordSearch,
            HybridRanker hybridRanker,
            ContextOptimizer contextOptimizer)
        {
            _queryExpander = queryExpander;
            _multiRetriever = multiRetriever;
            _rrfRanker = rrfRanker;
            _keywordSearch = keywordSearch;
            _hybridRanker = hybridRanker;
            _contextOptimizer = contextOptimizer;
        }

        public async Task<List<RetrievedChunk>>
            RetrieveAsync(string question)
        {
            var expandedQueries =
                await _queryExpander
                    .ExpandAsync(question);

            var retrievalResults =
                await _multiRetriever
                    .RetrieveAsync(expandedQueries);

            var rrfResults =
                _rrfRanker
                    .Rank(retrievalResults);

            return _contextOptimizer
                .Optimize(rrfResults);
        }
    }
}
