namespace Learnova.Infrastructure.Configuration
{
    public sealed class AdminBootstrapSettings
    {
        public const string SectionName = "AdminBootstrap";

        public string Email { get; init; } = string.Empty;

        public string Password { get; init; } = string.Empty;
    }
}
