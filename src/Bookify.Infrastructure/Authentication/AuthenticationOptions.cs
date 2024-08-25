namespace Bookify.Infrastructure.Authentication;

public sealed class AuthenticationOptions
{
    public const string SectionName = "Authentication";
    public string Audience { get; init; } = string.Empty;
    public string MetadataUrl { get; set; } = string.Empty;
    public bool RequireHttpsMetadata { get; init; }
    public string Issuer { get; set; } = string.Empty;
}