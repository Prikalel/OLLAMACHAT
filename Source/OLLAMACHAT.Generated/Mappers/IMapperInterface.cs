namespace OLLAMACHAT.Generated.Mappers;

[Mapper]
public interface IMapperInterface
{
    ParserRequest Map(ParserRequestDto dto);
    ParseResultDto Map(ParseResult result);
    ErrorResponseDto Map(ErrorResponse error);
    ResolveImportRequest Map(ResolveImportRequestDto dto);
}
