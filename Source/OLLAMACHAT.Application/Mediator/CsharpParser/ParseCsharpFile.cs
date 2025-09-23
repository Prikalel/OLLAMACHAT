namespace VelikiyPrikalel.OLLAMACHAT.Application.Mediator.CsharpParser;

using ParseCsharpFileResponse = (ParseResult result, ErrorResponse? error);

public sealed class ParseCsharpFile
{
    /// <summary>
    /// Запрос.
    /// </summary>
    public sealed record Query(ParserRequest request) : IRequest<ParseCsharpFileResponse>;

    /// <inheritdoc />
    public sealed class Handler() : IRequestHandler<Query, ParseCsharpFileResponse>
    {
        /// <inheritdoc />
        public async ValueTask<ParseCsharpFileResponse> Handle(Query request, CancellationToken cancellationToken)
        {
            string exampleJson = null;
            exampleJson = "{\n  \"relationships\" : [ {\n    \"targetFile\" : \"src/Interfaces/IUserRepository.cs\",\n    \"from\" : \"UserService\",\n    \"to\" : \"IUserRepository\",\n    \"type\" : \"implements\"\n  }, {\n    \"targetFile\" : \"src/Interfaces/IUserRepository.cs\",\n    \"from\" : \"UserService\",\n    \"to\" : \"IUserRepository\",\n    \"type\" : \"implements\"\n  } ],\n  \"metadata\" : {\n    \"loc\" : 245,\n    \"namespace\" : \"MyApp.Services\",\n    \"complexityScore\" : 8\n  },\n  \"entities\" : [ {\n    \"children\" : [ null, null ],\n    \"decorators\" : [ {\n      \"name\" : \"ApiController\",\n      \"arguments\" : [ \"\\\"api/[controller]\\\"\" ]\n    }, {\n      \"name\" : \"ApiController\",\n      \"arguments\" : [ \"\\\"api/[controller]\\\"\" ]\n    } ],\n    \"name\" : \"GetUserAsync\",\n    \"inheritance\" : {\n      \"interfaces\" : [ \"IUserService\" ],\n      \"baseClasses\" : [ \"ControllerBase\" ]\n    },\n    \"location\" : {\n      \"start\" : {\n        \"line\" : 12,\n        \"column\" : 19,\n        \"index\" : 156\n      }\n    },\n    \"importData\" : {\n      \"source\" : \"MyApp.Services\"\n    },\n    \"type\" : \"method\",\n    \"modifiers\" : [ \"public\", \"async\" ],\n    \"parameters\" : [ {\n      \"defaultValue\" : \"defaultValue\",\n      \"name\" : \"userId\",\n      \"optional\" : false,\n      \"type\" : \"int\"\n    }, {\n      \"defaultValue\" : \"defaultValue\",\n      \"name\" : \"userId\",\n      \"optional\" : false,\n      \"type\" : \"int\"\n    } ],\n    \"returnType\" : \"Task<User>\"\n  }, {\n    \"children\" : [ null, null ],\n    \"decorators\" : [ {\n      \"name\" : \"ApiController\",\n      \"arguments\" : [ \"\\\"api/[controller]\\\"\" ]\n    }, {\n      \"name\" : \"ApiController\",\n      \"arguments\" : [ \"\\\"api/[controller]\\\"\" ]\n    } ],\n    \"name\" : \"GetUserAsync\",\n    \"inheritance\" : {\n      \"interfaces\" : [ \"IUserService\" ],\n      \"baseClasses\" : [ \"ControllerBase\" ]\n    },\n    \"location\" : {\n      \"start\" : {\n        \"line\" : 12,\n        \"column\" : 19,\n        \"index\" : 156\n      }\n    },\n    \"importData\" : {\n      \"source\" : \"MyApp.Services\"\n    },\n    \"type\" : \"method\",\n    \"modifiers\" : [ \"public\", \"async\" ],\n    \"parameters\" : [ {\n      \"defaultValue\" : \"defaultValue\",\n      \"name\" : \"userId\",\n      \"optional\" : false,\n      \"type\" : \"int\"\n    }, {\n      \"defaultValue\" : \"defaultValue\",\n      \"name\" : \"userId\",\n      \"optional\" : false,\n      \"type\" : \"int\"\n    } ],\n    \"returnType\" : \"Task<User>\"\n  } ],\n  \"filePath\" : \"src/Services/UserService.cs\",\n  \"language\" : \"csharp\",\n  \"errors\" : [ {\n    \"severity\" : \"error\",\n    \"message\" : \"CS0103: The name 'userRepo' does not exist in the current context\"\n  }, {\n    \"severity\" : \"error\",\n    \"message\" : \"CS0103: The name 'userRepo' does not exist in the current context\"\n  } ],\n  \"contentHash\" : \"e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855\",\n  \"parseTimeMs\" : 156\n}";

            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<ParseResult>(exampleJson)
            : default(ParseResult);            //TODO: Change the data returned
            return (example, null);
        }
    }
}
