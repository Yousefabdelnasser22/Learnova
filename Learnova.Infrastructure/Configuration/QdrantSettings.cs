namespace Learnova.Infrastructure.Configuration
{
    public sealed class QdrantSettings
    {
        public const string SectionName = "Qdrant";

        public string Host { get; init; } = string.Empty;

        public int Port { get; init; } = 6334;

        public string ApiKey { get; init; } = string.Empty;
    }
}
