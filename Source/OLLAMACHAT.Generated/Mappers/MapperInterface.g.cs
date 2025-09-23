namespace OLLAMACHAT.Generated.Mappers
{
    public partial class MapperInterface : OLLAMACHAT.Generated.Mappers.IMapperInterface
    {
        public VelikiyPrikalel.OLLAMACHAT.Application.Models.ParserRequest Map(OLLAMACHAT.Generated.Models.ParserRequestDto p1)
        {
            return p1 == null ? null : new VelikiyPrikalel.OLLAMACHAT.Application.Models.ParserRequest(p1.FilePath, p1.RepoPath, p1.Options == null ? null : new VelikiyPrikalel.OLLAMACHAT.Application.Models.ParserOptions(p1.Options.ExtractReferences, p1.Options.ExtractInheritance, p1.Options.DetectPatterns, p1.Options.MaxDepth) {}) {Options = p1.Options == null ? null : new VelikiyPrikalel.OLLAMACHAT.Application.Models.ParserOptions(p1.Options.ExtractReferences, p1.Options.ExtractInheritance, p1.Options.DetectPatterns, p1.Options.MaxDepth) {}};
        }
        public OLLAMACHAT.Generated.Models.ParseResultDto Map(VelikiyPrikalel.OLLAMACHAT.Application.Models.ParseResult p2)
        {
            return p2 == null ? null : new OLLAMACHAT.Generated.Models.ParseResultDto()
            {
                FilePath = p2.FilePath,
                Language = (OLLAMACHAT.Generated.Models.ParseResultDto.LanguageEnumDto?)(OLLAMACHAT.Generated.Models.ParseResultDto.LanguageEnumDto)p2.Language,
                Entities = funcMain1(p2.Entities),
                Relationships = funcMain11(p2.Relationships),
                ContentHash = p2.ContentHash,
                ParseTimeMs = p2.ParseTimeMs,
                Errors = funcMain12(p2.Errors),
                Metadata = p2.Metadata == null ? null : new OLLAMACHAT.Generated.Models.FileMetadataDto()
                {
                    Loc = p2.Metadata.Loc,
                    ComplexityScore = p2.Metadata.ComplexityScore,
                    Namespace = p2.Metadata.Namespace
                }
            };
        }
        public OLLAMACHAT.Generated.Models.ErrorResponseDto Map(VelikiyPrikalel.OLLAMACHAT.Application.Models.ErrorResponse p15)
        {
            return p15 == null ? null : new OLLAMACHAT.Generated.Models.ErrorResponseDto()
            {
                Code = p15.Code,
                Message = p15.Message,
                Details = p15.Details
            };
        }
        public VelikiyPrikalel.OLLAMACHAT.Application.Models.ResolveImportRequest Map(OLLAMACHAT.Generated.Models.ResolveImportRequestDto p16)
        {
            return p16 == null ? null : new VelikiyPrikalel.OLLAMACHAT.Application.Models.ResolveImportRequest(p16.ImportPath, p16.FilePath, p16.RepoPath) {};
        }
        
        private System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParsedEntityDto> funcMain1(System.Collections.Generic.List<VelikiyPrikalel.OLLAMACHAT.Application.Models.ParsedEntity> p3)
        {
            if (p3 == null)
            {
                return null;
            }
            System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParsedEntityDto> result = new System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParsedEntityDto>(p3.Count);
            
            int i = 0;
            int len = p3.Count;
            
            while (i < len)
            {
                VelikiyPrikalel.OLLAMACHAT.Application.Models.ParsedEntity item = p3[i];
                result.Add(funcMain2(item));
                i++;
            }
            return result;
            
        }
        
        private System.Collections.Generic.List<OLLAMACHAT.Generated.Models.RelationshipDto> funcMain11(System.Collections.Generic.List<VelikiyPrikalel.OLLAMACHAT.Application.Models.Relationship> p13)
        {
            if (p13 == null)
            {
                return null;
            }
            System.Collections.Generic.List<OLLAMACHAT.Generated.Models.RelationshipDto> result = new System.Collections.Generic.List<OLLAMACHAT.Generated.Models.RelationshipDto>(p13.Count);
            
            int i = 0;
            int len = p13.Count;
            
            while (i < len)
            {
                VelikiyPrikalel.OLLAMACHAT.Application.Models.Relationship item = p13[i];
                result.Add(item == null ? null : new OLLAMACHAT.Generated.Models.RelationshipDto()
                {
                    From = item.From,
                    To = item.To,
                    Type = (OLLAMACHAT.Generated.Models.RelationshipDto.TypeEnumDto?)(OLLAMACHAT.Generated.Models.RelationshipDto.TypeEnumDto)item.Type,
                    TargetFile = item.TargetFile,
                    Location = item.Location == null ? null : new OLLAMACHAT.Generated.Models.LocationDto()
                    {
                        Start = item.Location.Start == null ? null : new OLLAMACHAT.Generated.Models.PositionDto()
                        {
                            Line = item.Location.Start.Line,
                            Column = item.Location.Start.Column,
                            Index = item.Location.Start.Index
                        },
                        End = item.Location.End == null ? null : new OLLAMACHAT.Generated.Models.PositionDto()
                        {
                            Line = item.Location.End.Line,
                            Column = item.Location.End.Column,
                            Index = item.Location.End.Index
                        }
                    }
                });
                i++;
            }
            return result;
            
        }
        
        private System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParseErrorDto> funcMain12(System.Collections.Generic.List<VelikiyPrikalel.OLLAMACHAT.Application.Models.ParseError> p14)
        {
            if (p14 == null)
            {
                return null;
            }
            System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParseErrorDto> result = new System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParseErrorDto>(p14.Count);
            
            int i = 0;
            int len = p14.Count;
            
            while (i < len)
            {
                VelikiyPrikalel.OLLAMACHAT.Application.Models.ParseError item = p14[i];
                result.Add(item == null ? null : new OLLAMACHAT.Generated.Models.ParseErrorDto()
                {
                    Message = item.Message,
                    Severity = (object)item.Severity == null ? null : Mapster.TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<OLLAMACHAT.Generated.Models.ParseErrorDto.SeverityEnumDto?>(((object)item.Severity).GetType()).Invoke((object)item.Severity),
                    Location = item.Location == null ? null : new OLLAMACHAT.Generated.Models.LocationDto()
                    {
                        Start = item.Location.Start == null ? null : new OLLAMACHAT.Generated.Models.PositionDto()
                        {
                            Line = item.Location.Start.Line,
                            Column = item.Location.Start.Column,
                            Index = item.Location.Start.Index
                        },
                        End = item.Location.End == null ? null : new OLLAMACHAT.Generated.Models.PositionDto()
                        {
                            Line = item.Location.End.Line,
                            Column = item.Location.End.Column,
                            Index = item.Location.End.Index
                        }
                    }
                });
                i++;
            }
            return result;
            
        }
        
        private OLLAMACHAT.Generated.Models.ParsedEntityDto funcMain2(VelikiyPrikalel.OLLAMACHAT.Application.Models.ParsedEntity p4)
        {
            return p4 == null ? null : new OLLAMACHAT.Generated.Models.ParsedEntityDto()
            {
                Name = p4.Name,
                Type = (OLLAMACHAT.Generated.Models.ParsedEntityDto.TypeEnumDto?)(OLLAMACHAT.Generated.Models.ParsedEntityDto.TypeEnumDto)p4.Type,
                Location = new OLLAMACHAT.Generated.Models.LocationDto()
                {
                    Start = p4.Location.Start == null ? null : new OLLAMACHAT.Generated.Models.PositionDto()
                    {
                        Line = p4.Location.Start.Line,
                        Column = p4.Location.Start.Column,
                        Index = p4.Location.Start.Index
                    },
                    End = p4.Location.End == null ? null : new OLLAMACHAT.Generated.Models.PositionDto()
                    {
                        Line = p4.Location.End.Line,
                        Column = p4.Location.End.Column,
                        Index = p4.Location.End.Index
                    }
                },
                Children = Mapster.TypeAdapter<System.Collections.Generic.List<VelikiyPrikalel.OLLAMACHAT.Application.Models.ParsedEntity>, System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParsedEntityDto>>.Map.Invoke(p4.Children),
                Modifiers = funcMain3(p4.Modifiers),
                Decorators = funcMain4(p4.Decorators),
                Inheritance = funcMain7(p4.Inheritance),
                ReturnType = p4.ReturnType,
                Parameters = funcMain10(p4.Parameters),
                ImportData = p4.ImportData == null ? null : new OLLAMACHAT.Generated.Models.ParsedEntityImportDataDto() {Source = p4.ImportData.Source}
            };
        }
        
        private System.Collections.Generic.List<string> funcMain3(System.Collections.Generic.List<string> p5)
        {
            if (p5 == null)
            {
                return null;
            }
            System.Collections.Generic.List<string> result = new System.Collections.Generic.List<string>(p5.Count);
            
            int i = 0;
            int len = p5.Count;
            
            while (i < len)
            {
                string item = p5[i];
                result.Add(item);
                i++;
            }
            return result;
            
        }
        
        private System.Collections.Generic.List<OLLAMACHAT.Generated.Models.DecoratorDto> funcMain4(System.Collections.Generic.List<VelikiyPrikalel.OLLAMACHAT.Application.Models.Decorator> p6)
        {
            if (p6 == null)
            {
                return null;
            }
            System.Collections.Generic.List<OLLAMACHAT.Generated.Models.DecoratorDto> result = new System.Collections.Generic.List<OLLAMACHAT.Generated.Models.DecoratorDto>(p6.Count);
            
            int i = 0;
            int len = p6.Count;
            
            while (i < len)
            {
                VelikiyPrikalel.OLLAMACHAT.Application.Models.Decorator item = p6[i];
                result.Add(funcMain5(item));
                i++;
            }
            return result;
            
        }
        
        private OLLAMACHAT.Generated.Models.ParsedEntityInheritanceDto funcMain7(VelikiyPrikalel.OLLAMACHAT.Application.Models.ParsedEntityInheritance p9)
        {
            return p9 == null ? null : new OLLAMACHAT.Generated.Models.ParsedEntityInheritanceDto()
            {
                BaseClasses = funcMain8(p9.BaseClasses),
                Interfaces = funcMain9(p9.Interfaces)
            };
        }
        
        private System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ModelParameterDto> funcMain10(System.Collections.Generic.List<VelikiyPrikalel.OLLAMACHAT.Application.Models.ModelParameter> p12)
        {
            if (p12 == null)
            {
                return null;
            }
            System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ModelParameterDto> result = new System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ModelParameterDto>(p12.Count);
            
            int i = 0;
            int len = p12.Count;
            
            while (i < len)
            {
                VelikiyPrikalel.OLLAMACHAT.Application.Models.ModelParameter item = p12[i];
                result.Add(item == null ? null : new OLLAMACHAT.Generated.Models.ModelParameterDto()
                {
                    Name = item.Name,
                    Type = item.Type,
                    Optional = item.Optional,
                    DefaultValue = item.DefaultValue
                });
                i++;
            }
            return result;
            
        }
        
        private OLLAMACHAT.Generated.Models.DecoratorDto funcMain5(VelikiyPrikalel.OLLAMACHAT.Application.Models.Decorator p7)
        {
            return p7 == null ? null : new OLLAMACHAT.Generated.Models.DecoratorDto()
            {
                Name = p7.Name,
                Arguments = funcMain6(p7.Arguments)
            };
        }
        
        private System.Collections.Generic.List<string> funcMain8(System.Collections.Generic.List<string> p10)
        {
            if (p10 == null)
            {
                return null;
            }
            System.Collections.Generic.List<string> result = new System.Collections.Generic.List<string>(p10.Count);
            
            int i = 0;
            int len = p10.Count;
            
            while (i < len)
            {
                string item = p10[i];
                result.Add(item);
                i++;
            }
            return result;
            
        }
        
        private System.Collections.Generic.List<string> funcMain9(System.Collections.Generic.List<string> p11)
        {
            if (p11 == null)
            {
                return null;
            }
            System.Collections.Generic.List<string> result = new System.Collections.Generic.List<string>(p11.Count);
            
            int i = 0;
            int len = p11.Count;
            
            while (i < len)
            {
                string item = p11[i];
                result.Add(item);
                i++;
            }
            return result;
            
        }
        
        private System.Collections.Generic.List<string> funcMain6(System.Collections.Generic.List<string> p8)
        {
            if (p8 == null)
            {
                return null;
            }
            System.Collections.Generic.List<string> result = new System.Collections.Generic.List<string>(p8.Count);
            
            int i = 0;
            int len = p8.Count;
            
            while (i < len)
            {
                string item = p8[i];
                result.Add(item);
                i++;
            }
            return result;
            
        }
    }
}