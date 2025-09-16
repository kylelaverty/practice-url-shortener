using System.Diagnostics.CodeAnalysis;

// Suppress requiring ConfigureAwait(false) in our ASP.NET Core code.
// There are two diagnostics to suppress, one from the compiler and one
// from the Visual Studio Threading Analyzer package.

// Suppress: https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca2007
[assembly: SuppressMessage(
    "Reliability",
    "CA2007",
    Justification = "ASP.NET Core doesn't use SynchronizationContexts so ConfigureAwait(false) doesn't do anything.")]

// Suppress: https://github.com/microsoft/vs-threading/blob/main/doc/analyzers/VSTHRD111.md
[assembly: SuppressMessage(
    "Usage",
    "VSTHRD111",
    Justification = "ASP.NET Core doesn't use SynchronizationContexts so ConfigureAwait(false) doesn't do anything.")]