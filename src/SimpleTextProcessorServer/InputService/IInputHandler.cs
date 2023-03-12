namespace SimpleTextProcessorServer.InputService;

public interface IInputHandler
{
    public Task HandleInput(string[] userInput);
}
