namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Services.Parser;

/// <summary>
/// Часть класса EntityService, содержащая константы и общие утилиты
/// </summary>
public partial class EntityService
{
    /// <summary>
    /// Максимальный размер файла в байтах для предупреждения о большом файле
    /// </summary>
    private const int MaxFileSizeWarningBytes = 100_000;

    /// <summary>
    /// Максимальная длина имени using директивы для предупреждения
    /// </summary>
    private const int MaxUsingNameLength = 200;

    /// <summary>
    /// Максимальное количество синтаксических ошибок для логирования
    /// </summary>
    private const int MaxSyntaxErrorsToLog = 5;

    /// <summary>
    /// Важные методы для включения при переопределении
    /// </summary>
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
            return false;

        return name.Any(c => !char.IsLetterOrDigit(c) && c != '_' && c != '`' && c != '.' &&
                            c != '[' && c != ']' && c != '<' && c != '>' && c != ',');
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