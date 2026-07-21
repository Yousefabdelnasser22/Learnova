using Qdrant.Client;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Infrastructure.Search
{
    public class QdrantInitializer
    {
        private readonly QdrantClient _qdrant;

        public QdrantInitializer(QdrantClient qdrant)
        {
            _qdrant = qdrant;
        }

        public async Task InitializeAsync()
        {
            var collections = await _qdrant.ListCollectionsAsync();

            if (!collections.Contains("courses"))
            {
                await _qdrant.CreateCollectionAsync("courses", new VectorParams
                {
                    Size = 3072,
                    Distance = Distance.Cosine
                });
            }
        }
    }
}
