namespace Learnova.Infrastructure.Configuration
{
    public sealed class GeminiSettings
    {
        public const string SectionName = "Gemini";

        public string ApiKey { get; init; } = string.Empty;

        public string EmbeddingModel { get; init; } = string.Empty;
    }
}
