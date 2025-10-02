namespace OLLAMACHAT.Generated.Mappers
{
    public partial class MapperInterface : OLLAMACHAT.Generated.Mappers.IMapperInterface
    {
        public VelikiyPrikalel.OLLAMACHAT.Application.Models.ParserRequest Map(OLLAMACHAT.Generated.Models.ParseRequest.ParserRequestDto p1)
        {
            return p1 == null ? null : new VelikiyPrikalel.OLLAMACHAT.Application.Models.ParserRequest(p1.FilePath, p1.RepoPath, p1.Options == null ? null : new VelikiyPrikalel.OLLAMACHAT.Application.Models.ParserOptions(p1.Options.ExtractFullExtractInheritance, p1.Options.MaxDepth, p1.Options.ExtractUsingStatementData) {}) {Options = p1.Options == null ? null : new VelikiyPrikalel.OLLAMACHAT.Application.Models.ParserOptions(p1.Options.ExtractFullExtractInheritance, p1.Options.MaxDepth, p1.Options.ExtractUsingStatementData) {}};
        }
        public OLLAMACHAT.Generated.Models.ParseResponse.ParseResultDto Map(VelikiyPrikalel.OLLAMACHAT.Application.Models.ParseResult p2)
        {
            return p2 == null ? null : new OLLAMACHAT.Generated.Models.ParseResponse.ParseResultDto()
            {
                ContentHash = p2.ContentHash,
                Entities = funcMain1(p2.Entities),
                Errors = funcMain13(p2.Errors),
                FilePath = p2.FilePath,
                Language = (OLLAMACHAT.Generated.Models.ParseResponse.ParseResultDto.LanguageEnumDto?)(OLLAMACHAT.Generated.Models.ParseResponse.ParseResultDto.LanguageEnumDto)p2.Language,
                ParseTimeMs = p2.ParseTimeMs,
                Relationships = funcMain14(p2.Relationships)
            };
        }
        public OLLAMACHAT.Generated.Models.ErrorResponseDto Map(VelikiyPrikalel.OLLAMACHAT.Application.Models.ErrorResponse p17)
        {
            return p17 == null ? null : new OLLAMACHAT.Generated.Models.ErrorResponseDto()
            {
                Code = p17.Code,
                Details = p17.Details,
                Message = p17.Message
            };
        }
        public VelikiyPrikalel.OLLAMACHAT.Application.Models.ResolveImportRequest Map(OLLAMACHAT.Generated.Models.ResolveImportRequestDto p18)
        {
            return p18 == null ? null : new VelikiyPrikalel.OLLAMACHAT.Application.Models.ResolveImportRequest(p18.ImportPath, p18.FilePath, p18.RepoPath) {};
        }
        
        private System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParseResponse.ParsedEntityDto> funcMain1(System.Collections.Generic.List<VelikiyPrikalel.OLLAMACHAT.Application.Models.ParsedEntity> p3)
        {
            if (p3 == null)
            {
                return null;
            }
            System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParseResponse.ParsedEntityDto> result = new System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParseResponse.ParsedEntityDto>(p3.Count);
            
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
        
        private System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParseErrorDto> funcMain13(System.Collections.Generic.List<VelikiyPrikalel.OLLAMACHAT.Application.Models.ParseError> p15)
        {
            if (p15 == null)
            {
                return null;
            }
            System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParseErrorDto> result = new System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParseErrorDto>(p15.Count);
            
            int i = 0;
            int len = p15.Count;
            
            while (i < len)
            {
                VelikiyPrikalel.OLLAMACHAT.Application.Models.ParseError item = p15[i];
                result.Add(item == null ? null : new OLLAMACHAT.Generated.Models.ParseErrorDto()
                {
                    Location = item.Location == null ? null : new OLLAMACHAT.Generated.Models.ParseResponse.LocationDto()
                    {
                        Start = item.Location.Start == null ? null : new OLLAMACHAT.Generated.Models.ParseResponse.PositionDto()
                        {
                            Column = item.Location.Start.Column,
                            Index = item.Location.Start.Index,
                            Line = item.Location.Start.Line
                        },
                        End = item.Location.End == null ? null : new OLLAMACHAT.Generated.Models.ParseResponse.PositionDto()
                        {
                            Column = item.Location.End.Column,
                            Index = item.Location.End.Index,
                            Line = item.Location.End.Line
                        }
                    },
                    Message = item.Message,
                    Severity = (object)item.Severity == null ? null : Mapster.TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<OLLAMACHAT.Generated.Models.ParseErrorDto.SeverityEnumDto?>(((object)item.Severity).GetType()).Invoke((object)item.Severity)
                });
                i++;
            }
            return result;
            
        }
        
        private System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParseResponse.RelationshipDto> funcMain14(System.Collections.Generic.List<VelikiyPrikalel.OLLAMACHAT.Application.Models.Relationship> p16)
        {
            if (p16 == null)
            {
                return null;
            }
            System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParseResponse.RelationshipDto> result = new System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParseResponse.RelationshipDto>(p16.Count);
            
            int i = 0;
            int len = p16.Count;
            
            while (i < len)
            {
                VelikiyPrikalel.OLLAMACHAT.Application.Models.Relationship item = p16[i];
                result.Add(item == null ? null : new OLLAMACHAT.Generated.Models.ParseResponse.RelationshipDto()
                {
                    FullNameFrom = item.FullNameFrom,
                    TargetDefinitionFilePath = item.TargetDefinitionFilePath,
                    FullNameTo = item.FullNameTo,
                    Type = (OLLAMACHAT.Generated.Models.ParseResponse.RelationshipDto.TypeEnumDto?)(OLLAMACHAT.Generated.Models.ParseResponse.RelationshipDto.TypeEnumDto)item.Type
                });
                i++;
            }
            return result;
            
        }
        
        private OLLAMACHAT.Generated.Models.ParseResponse.ParsedEntityDto funcMain2(VelikiyPrikalel.OLLAMACHAT.Application.Models.ParsedEntity p4)
        {
            return p4 == null ? null : new OLLAMACHAT.Generated.Models.ParseResponse.ParsedEntityDto()
            {
                Children = Mapster.TypeAdapter<System.Collections.Generic.List<VelikiyPrikalel.OLLAMACHAT.Application.Models.ParsedEntity>, System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParseResponse.ParsedEntityDto>>.Map.Invoke(p4.Children),
                Attributes = funcMain3(p4.Attributes),
                UsingStatementData = p4.UsingStatementData == null ? null : new OLLAMACHAT.Generated.Models.ParseResponse.UsingStatementDataDto() {Source = p4.UsingStatementData.Source},
                Inheritance = funcMain6(p4.Inheritance),
                Location = new OLLAMACHAT.Generated.Models.ParseResponse.LocationDto()
                {
                    Start = p4.Location.Start == null ? null : new OLLAMACHAT.Generated.Models.ParseResponse.PositionDto()
                    {
                        Column = p4.Location.Start.Column,
                        Index = p4.Location.Start.Index,
                        Line = p4.Location.Start.Line
                    },
                    End = p4.Location.End == null ? null : new OLLAMACHAT.Generated.Models.ParseResponse.PositionDto()
                    {
                        Column = p4.Location.End.Column,
                        Index = p4.Location.End.Index,
                        Line = p4.Location.End.Line
                    }
                },
                Modifiers = funcMain11(p4.Modifiers),
                SimpleName = p4.SimpleName,
                Parameters = funcMain12(p4.Parameters),
                ReturnType = p4.ReturnType,
                FullName = p4.FullName,
                Type = (OLLAMACHAT.Generated.Models.ParseResponse.ParsedEntityDto.TypeEnumDto?)(OLLAMACHAT.Generated.Models.ParseResponse.ParsedEntityDto.TypeEnumDto)p4.Type
            };
        }
        
        private System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParseResponse.AttributeDto> funcMain3(System.Collections.Generic.List<VelikiyPrikalel.OLLAMACHAT.Application.Models.Attribute> p5)
        {
            if (p5 == null)
            {
                return null;
            }
            System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParseResponse.AttributeDto> result = new System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParseResponse.AttributeDto>(p5.Count);
            
            int i = 0;
            int len = p5.Count;
            
            while (i < len)
            {
                VelikiyPrikalel.OLLAMACHAT.Application.Models.Attribute item = p5[i];
                result.Add(funcMain4(item));
                i++;
            }
            return result;
            
        }
        
        private OLLAMACHAT.Generated.Models.ParseResponse.ParsedEntityInheritanceDto funcMain6(VelikiyPrikalel.OLLAMACHAT.Application.Models.ParsedEntityInheritance p8)
        {
            return p8 == null ? null : new OLLAMACHAT.Generated.Models.ParseResponse.ParsedEntityInheritanceDto()
            {
                DirectBaseClasses = funcMain7(p8.DirectBaseClasses),
                DirectInterfaces = funcMain8(p8.DirectInterfaces),
                AllBaseClasses = funcMain9(p8.AllBaseClasses),
                AllInterfaces = funcMain10(p8.AllInterfaces)
            };
        }
        
        private System.Collections.Generic.List<string> funcMain11(System.Collections.Generic.List<string> p13)
        {
            if (p13 == null)
            {
                return null;
            }
            System.Collections.Generic.List<string> result = new System.Collections.Generic.List<string>(p13.Count);
            
            int i = 0;
            int len = p13.Count;
            
            while (i < len)
            {
                string item = p13[i];
                result.Add(item);
                i++;
            }
            return result;
            
        }
        
        private System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParseResponse.ModelParameterDto> funcMain12(System.Collections.Generic.List<VelikiyPrikalel.OLLAMACHAT.Application.Models.ModelParameter> p14)
        {
            if (p14 == null)
            {
                return null;
            }
            System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParseResponse.ModelParameterDto> result = new System.Collections.Generic.List<OLLAMACHAT.Generated.Models.ParseResponse.ModelParameterDto>(p14.Count);
            
            int i = 0;
            int len = p14.Count;
            
            while (i < len)
            {
                VelikiyPrikalel.OLLAMACHAT.Application.Models.ModelParameter item = p14[i];
                result.Add(item == null ? null : new OLLAMACHAT.Generated.Models.ParseResponse.ModelParameterDto()
                {
                    DefaultValue = item.DefaultValue,
                    Name = item.Name,
                    Nullable = item.Nullable,
                    FullTypeName = item.FullTypeName
                });
                i++;
            }
            return result;
            
        }
        
        private OLLAMACHAT.Generated.Models.ParseResponse.AttributeDto funcMain4(VelikiyPrikalel.OLLAMACHAT.Application.Models.Attribute p6)
        {
            return p6 == null ? null : new OLLAMACHAT.Generated.Models.ParseResponse.AttributeDto()
            {
                Arguments = funcMain5(p6.Arguments),
                Name = p6.Name
            };
        }
        
        private System.Collections.Generic.List<string> funcMain7(System.Collections.Generic.List<string> p9)
        {
            if (p9 == null)
            {
                return null;
            }
            System.Collections.Generic.List<string> result = new System.Collections.Generic.List<string>(p9.Count);
            
            int i = 0;
            int len = p9.Count;
            
            while (i < len)
            {
                string item = p9[i];
                result.Add(item);
                i++;
            }
            return result;
            
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
        
        private System.Collections.Generic.List<string> funcMain10(System.Collections.Generic.List<string> p12)
        {
            if (p12 == null)
            {
                return null;
            }
            System.Collections.Generic.List<string> result = new System.Collections.Generic.List<string>(p12.Count);
            
            int i = 0;
            int len = p12.Count;
            
            while (i < len)
            {
                string item = p12[i];
                result.Add(item);
                i++;
            }
            return result;
            
        }
        
        private System.Collections.Generic.List<string> funcMain5(System.Collections.Generic.List<string> p7)
        {
            if (p7 == null)
            {
                return null;
            }
            System.Collections.Generic.List<string> result = new System.Collections.Generic.List<string>(p7.Count);
            
            int i = 0;
            int len = p7.Count;
            
            while (i < len)
            {
                string item = p7[i];
                result.Add(item);
                i++;
            }
            return result;
            
        }
    }
}