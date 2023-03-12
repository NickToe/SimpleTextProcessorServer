namespace SimpleTextProcessorServer.InputService;

public sealed class InputReader
{
    private readonly IInputHandler _inputHandler;

    public InputReader()
    {
        _inputHandler = new InputHandler();
    }

    /* Main loop for Dictionary related operations */
    public async Task WaitForInput()
    {
        string? userInput = string.Empty;
        while (true)
        {
            userInput = Console.ReadLine();
            if (string.IsNullOrEmpty(userInput)) continue;
            try
            {
                await _inputHandler.HandleInput(userInput.TrimEnd(' ').Split(' ').ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WaitForInput(): {ex.GetType()}: {ex.Message}");
            }
        }
    }
}
