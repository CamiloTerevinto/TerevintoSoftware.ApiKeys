# TerevintoSoftware.AspNetCore.Authentication.ApiKeys [![Coverage Status](https://coveralls.io/repos/github/CamiloTerevinto/TerevintoSoftware.ApiKeys/badge.svg?branch=master)](https://coveralls.io/github/CamiloTerevinto/TerevintoSoftware.ApiKeys?branch=master)

This solution contains a set of classes and interfaces to add API Keys authentication to an ASP.NET Core application.  
This solution is split into two packages:

| Package | Purpose |
| ------- | ------- |
| [TerevintoSoftware.AspNetCore.Authentication.ApiKeys](https://www.nuget.org/packages/TerevintoSoftware.AspNetCore.Authentication.ApiKeys/) | Contains main logic, depends on ASP.NET Core. |
| [TerevintoSoftware.AspNetCore.Authentication.ApiKeys.Abstractions](https://www.nuget.org/packages/TerevintoSoftware.AspNetCore.Authentication.ApiKeys.Abstractions/) | Contains interfaces used by the main package. |

### 1. `TerevintoSoftware.AspNetCore.Authentication.ApiKeys`

This is the main package that contains the main classes and support for ASP.NET Core.  
Since there are dependencies on classes from the `Microsoft.AspNetCore.Authetication` namespace, this contains only logic that should not change a lot under real usage.  
Default implementations of some of the interfaces provided in the Abstractions package are in this package.

### 2. `TerevintoSoftware.AspNetCore.Authentication.ApiKeys.Abstractions`

This package contains interfaces that are needed by the main package:

* `IApiKeyFactory` -> used to generate new API Keys. Default implementation provided in main package.
* `IApiKeysCacheService` -> used to add and remove from cache the API Keys. This needs to be implemented by the consumer of the package.
* `IClaimsPrincipalFactory` -> used to create `ClaimsPrincipal`s once a key is succesfully validated. Default implementation provided in main package.

## Sample usage

A runnable sample of this package is available [in my Blog repository](https://github.com/CamiloTerevinto/Blog/tree/main/Samples/Simple%20and%20secure%20custom%20API%20Keys%20using%20ASP.NET%20Core).

The default implementations can be injected as follows:

```c#
builder.Services
    .AddDefaultApiKeyGenerator(new ApiKeyGenerationOptions
    {
        KeyPrefix = "CT-",
        GenerateUrlSafeKeys = true,
        LengthOfKey = 36
    })
    .AddDefaultClaimsPrincipalFactory()
    .AddSingleton<IApiKeysCacheService, CacheService>() // CacheService is not provided
    .AddApiKeys(options => 
    { 
        options.InvalidApiKeyLog = (LogLevel.Warning, "Someone attempted to use an invalid API Key: {ApiKey}");  
    }, true);
```

An example key generated by that configuration is: `CT-mTbC4r1Eh7wvXrXE1UDl18NGH1fRzcrRz`.

## How to build

* Install Visual Studio 2022 (.NET 6 required), if needed.
* Install git, if needed.
* Clone this repository.
* Build from Visual Studio or through `dotnet build`.

## Bug reports and feature requests

Please use the [issue tracker](https://github.com/CamiloTerevinto/TerevintoSoftware.ApiKeys/issues) and ensure your question/feedback was not previously reported.
