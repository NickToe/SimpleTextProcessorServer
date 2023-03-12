using SimpleTextProcessorProtocol;
using SimpleTextProcessorServer.Services;

namespace SimpleTextProcessorServer.Server;

public sealed class CommandHandler : ICommandHandler
{

    public async Task<Response> HandleRequest(string[] request) => request[0] switch
    {
        "get" => await HandleGetCommand(request.Skip(1)),
        _ => HandleUnknownCommand(request[0]),
    };


    private async Task<Response> HandleGetCommand(IEnumerable<string> message)
    {
        string? data = message.FirstOrDefault();
        if (string.IsNullOrEmpty(data))
        {
            return new Response(ResponseStatusCode.INVALID_DATA, "Command specific data not found");
        }

        DictionaryService service = new();
        string formattedData = await service.GetDictionary(data);
        if(string.IsNullOrEmpty(formattedData))
        {
            return new Response(ResponseStatusCode.NO_DATA, "No matches in database");
        }

        return new Response(ResponseStatusCode.OK, formattedData);
    }


    private Response HandleUnknownCommand(string unknownCommand)
    {
        return new(ResponseStatusCode.INVALID_CMD, $"Unknown command '{unknownCommand}'");
    }
}
