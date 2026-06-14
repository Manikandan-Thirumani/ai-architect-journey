using AiLearningApi.Models;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace AiLearningApi.Services;

public class QdrantVectorStoreService
{
    private readonly QdrantClient _client;

    private const string CollectionName =
        "banking_policies";

    public QdrantVectorStoreService()
    {
        _client = new QdrantClient(
            host: "localhost",
            port: 6334); 
    }

    // =====================================
    // CREATE COLLECTION
    // =====================================

    public async Task CreateCollectionAsync(
        int vectorSize)
    {
        try
        {
            var collections =
                await _client
                    .ListCollectionsAsync();

            if (collections.Any(
                x => x == CollectionName))
            {
                Console.WriteLine(
                    $"Collection '{CollectionName}' already exists.");

                return;
            }

            await _client
                .CreateCollectionAsync(
                    CollectionName,
                    new VectorParams
                    {
                        Size =
                            (ulong)vectorSize,

                        Distance =
                            Distance.Cosine
                    });

            Console.WriteLine(
                $"Collection '{CollectionName}' created.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"CreateCollectionAsync Error: {ex.Message}");

            throw;
        }
    }

    // =====================================
    // INSERT VECTOR
    // =====================================

    public async Task InsertAsync(
        string id,
        float[] embedding,
        string content,
        string policyName,
        string sourceDocument)
    {
        try
        {
            await _client.UpsertAsync(
                CollectionName,
                new List<PointStruct>
                {
                    new()
                    {
                        Id = new PointId
                        {
                            Uuid = id
                        },

                        Vectors = embedding,

                        Payload =
                        {
                            ["content"] =
                                content,

                            ["policy"] =
                                policyName,

                            ["source"] =
                                sourceDocument
                        }
                    }
                });
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"InsertAsync Error: {ex.Message}");

            throw;
        }
    }

    // =====================================
    // SEARCH
    // =====================================

    public async Task<List<RetrievedChunk>>
        SearchAsync(
            float[] queryEmbedding,
            int topK = 5)
    {
        try
        {
            var results =
                await _client.SearchAsync(
                    collectionName:
                        CollectionName,

                    vector:
                        queryEmbedding,

                    limit:
                        (ulong)topK);

            return results
                .Select(x =>
                    new RetrievedChunk
                    {
                        Content =
                            x.Payload[
                                "content"]
                                .StringValue,

                        PolicyName =
                            x.Payload[
                                "policy"]
                                .StringValue,

                        SourceDocument =
                            x.Payload[
                                "source"]
                                .StringValue,

                        Score =
                            x.Score
                    })
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"SearchAsync Error: {ex.Message}");

            throw;
        }
    }
    public async Task<List<RetrievedChunk>>
    GetAllDocumentsAsync()
    {
        var results =
            await _client.ScrollAsync(
                collectionName:
                    CollectionName,

                limit: 100);

        return results.Result
            .Select(x =>
                new RetrievedChunk
                {
                    Content =
                        x.Payload["content"]
                            .StringValue,

                    PolicyName =
                        x.Payload["policy"]
                            .StringValue,

                    SourceDocument =
                        x.Payload["source"]
                            .StringValue,

                    Score = 0
                })
            .ToList();
    }
}