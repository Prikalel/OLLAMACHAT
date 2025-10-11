namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

/// <summary>
/// Часть класса EntityService, содержащая константы и общие утилиты
/// </summary>
public partial class EntityService
{
    private static readonly Regex unityEventRegex = new(@"UnityEvent<.*>", RegexOptions.Compiled);
    private static readonly string[] unityEventNames = { "UnityEvent", "UnityEvent<T>", "UnityEvent<T0, T1>", "UnityEvent<T0, T1, T2>", "UnityEvent<T0, T1, T2, T3>" };
    private const int MaxFileSizeWarningBytes = 100_000;
    private static readonly string[] ImportantOverrideMethods =
    {
        "ToString", "Equals", "GetHashCode", "Finalize"
    };

    /// <summary>
    /// Проверяет, содержит ли строка специальные символы (кроме допустимых)
    /// </summary>
    /// <param name="name">Имя для проверки</param>
    /// <returns>True, если содержатся недопустимые символы</returns>
    private static bool ContainsSpecialCharacters(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        const string AllowedSymbols = "_`.[]<>,";
        return name.Any(c => !char.IsLetterOrDigit(c) && !AllowedSymbols.Contains(c));
    }

    /// <summary>
    /// Проверяет, является ли символ важным переопределенным методом
    /// </summary>
    /// <param name="symbol">Символ для проверки</param>
    /// <returns>True, если это важный переопределенный метод</returns>
    private static bool IsImportantOverrideMethod(ISymbol symbol)
    {
        if (symbol is IMethodSymbol methodSymbol)
        {
            return ImportantOverrideMethods.Contains(methodSymbol.Name);
        }
        return false;
    }
}
