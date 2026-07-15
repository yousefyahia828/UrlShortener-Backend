using System.Reflection;

namespace UrlShortener.Infrastructure;

public static class InfrastructureAssemblyReference
{
    public static readonly Assembly Assembly
        = typeof(InfrastructureAssemblyReference).Assembly;
}
