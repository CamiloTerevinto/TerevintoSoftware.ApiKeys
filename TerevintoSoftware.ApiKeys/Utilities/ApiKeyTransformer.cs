using Microsoft.AspNetCore.OpenApi;
using System.Diagnostics.CodeAnalysis;
using TerevintoSoftware.AspNetCore.Authentication.ApiKeys;

namespace Microsoft.OpenApi;

[ExcludeFromCodeCoverage]
public class ApiKeyTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var securitySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            [ApiKeyAuthenticationOptions.DefaultScheme] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Scheme = ApiKeyAuthenticationOptions.DefaultScheme,
                Name = ApiKeyAuthenticationOptions.HeaderName
            }
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = securitySchemes;
        document.Security = [new OpenApiSecurityRequirement() { { new OpenApiSecuritySchemeReference(ApiKeyAuthenticationOptions.DefaultScheme), [] } }];
        document.SetReferenceHostDocument();

        return Task.CompletedTask;
    }
}
