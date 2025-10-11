namespace OLLAMACHAT.Generated.Mappers;

/// <summary>
/// Маппер.
/// </summary>
[Mapper]
public interface IMapperInterface
{
    /// <summary>
    /// <see cref="ParserRequestDto"/> -> <see cref="ParserRequest"/>.
    /// </summary>
    /// <param name="dto">ParserRequestDto.</param>
    /// <returns>ParserRequest.</returns>
    ParserRequest Map(ParserRequestDto dto);

    /// <summary>
    /// <see cref="ParseResult"/> -> <see cref="ParseResultDto"/>.
    /// </summary>
    /// <param name="result"><see cref="ParseResult"/>.</param>
    /// <returns><see cref="ParseResultDto"/>.</returns>
    ParseResultDto Map(ParseResult result);

    /// <summary>
    /// <see cref="ErrorResponse"/> -> <see cref="ErrorResponseDto"/>.
    /// </summary>
    /// <param name="error">ErrorResponse.</param>
    /// <returns>ErrorResponseDto.</returns>
    ErrorResponseDto Map(ErrorResponse error);

    /// <summary>
    /// <see cref="ResolveImportRequestDto"/> -> <see cref="ResolveImportRequest"/>.
    /// </summary>
    /// <param name="dto">ResolveImportRequestDto.</param>
    /// <returns>ResolveImportRequest.</returns>
    ResolveImportRequest Map(ResolveImportRequestDto dto);
}
