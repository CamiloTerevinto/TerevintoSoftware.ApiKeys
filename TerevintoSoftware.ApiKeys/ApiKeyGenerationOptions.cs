namespace TerevintoSoftware.AspNetCore.Authentication.ApiKeys;

/// <summary>
/// Contains options used to generate API Keys by the default factory.
/// </summary>
public class ApiKeyGenerationOptions
{
    /// <summary>
    /// Gets or sets a prefix for the keys. For example: "EX-"
    /// </summary>
    public string? KeyPrefix { get; set; }

    /// <summary>
    /// Gets or sets the desider final length for an API key.
    /// </summary>
    public int LengthOfKey { get; set; }

    /// <summary>
    /// Gets or sets whether to generate URL-safe keys. Base64 is used if false.
    /// </summary>
    public bool GenerateUrlSafeKeys { get; set; }

    /// <summary>
    /// Validates the configuration, to allow settings to be mapped with Microsoft.Extensions.Configuration.
    /// </summary>
    /// <exception cref="InvalidOperationException">When <see cref="ByteCountToGenerate"/> or <see cref="LengthOfKey"/> are below 1.</exception>
    public void Validate()
    {
        KeyPrefix ??= "";

        if (LengthOfKey < 1)
        {
            throw new InvalidOperationException("A positive, non-zero length of key is required.");
        }

        if (LengthOfKey < KeyPrefix.Length)
        {
            throw new InvalidOperationException("The length of the key must be bigger than the key prefix.");
        }
    }
}
