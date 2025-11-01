namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Cache;

/// <summary>
/// Наберёшь.
/// </summary>
/// <param name="FullName">ГОЙДА.</param>
/// <param name="BaseClassnames">ГОЙДА.</param>
/// <param name="AllInterfaces">ГОЙДА.</param>
/// <param name="FilePath">ГОЙДА.</param>
public record NamedTypeSymbol(
    string FullName,
    string[] BaseClassnames,
    string[] AllInterfaces,
    string? FilePath)
{
    /// <summary>
    /// Фабрика из <see cref="INamedTypeSymbol"/>.
    /// </summary>
    /// <param name="typeSymbol"><see cref="INamedTypeSymbol"/>.</param>
    /// <returns>ГОЙДА.</returns>
    public static NamedTypeSymbol Create(INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol == null)
        {
            throw new ArgumentNullException();
        }

        List<string> baseClassnames = [];
        List<string> allInterfaces = [];

        INamedTypeSymbol? baseType = typeSymbol.BaseType;
        while (baseType != null)
        {
            baseClassnames.Add(baseType.GetFullName());

            baseType = baseType.BaseType;
        }

        foreach (INamedTypeSymbol interfaceType in typeSymbol.AllInterfaces)
        {
            allInterfaces.Add(interfaceType.GetFullName());
        }

        return new(typeSymbol.GetFullName(),
            baseClassnames.ToArray(),
            allInterfaces.ToArray(),
            GetFilePathForTypeInternal(typeSymbol));
    }

    private static string? GetFilePathForTypeInternal(INamedTypeSymbol typeSymbol)
    {
        ImmutableArray<SyntaxReference> typeDeclarations = typeSymbol.DeclaringSyntaxReferences;
        if (typeDeclarations.Length == 0)
        {
            return null;
        }

        SyntaxNode syntaxNode = typeDeclarations[0].GetSyntax();
        SyntaxTree syntaxTree = syntaxNode.SyntaxTree;
        string filePath = syntaxTree.FilePath;

        if (string.IsNullOrEmpty(filePath))
        {
            return null;
        }

        // Возвращаем абсолютный путь
        return Path.GetFullPath(filePath);
    }
}
