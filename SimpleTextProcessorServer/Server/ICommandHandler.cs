using SimpleTextProcessorProtocol;

namespace SimpleTextProcessorServer.Server;

public interface ICommandHandler
{
    public Task<Response> HandleRequest(string[] request);
}
