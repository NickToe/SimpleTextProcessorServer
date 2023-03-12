using SimpleTextProcessorServer.Services;

namespace SimpleTextProcessorServer.InputService;

public sealed class InputHandler : IInputHandler
{
    public async Task HandleInput(string[] userInput)
    {
        switch (userInput[0])
        {
            case "create":
                await CreateDictionary(userInput.Skip(1));
                break;
            case "update":
                await UpdateDictionary(userInput.Skip(1));
                break;
            case "clear":
                await ClearDictionary(userInput.Skip(1));
                break;
            case "get":
                await GetDictionary(userInput.Skip(1));
                break;
            default:
                Console.WriteLine($"Command {userInput[0]} is not supported{Environment.NewLine}");
                break;
        }
    }


    private async Task CreateDictionary(IEnumerable<string> message)
    {
        string? filename = message.FirstOrDefault();
        if (string.IsNullOrEmpty(filename))
        {
            Console.WriteLine($"Failed to get filename from command line arguments{Environment.NewLine}");
            return;
        }
        ParserService parserService = new();
        var list = parserService.ParseTextFile(filename);
        DictionaryService dictionaryService = new();
        await dictionaryService.CreateDictionary(list);
    }


    private async Task UpdateDictionary(IEnumerable<string> message)
    {
        string? filename = message.FirstOrDefault();
        if (string.IsNullOrEmpty(filename))
        {
            Console.WriteLine($"Failed to get filename from command line arguments{Environment.NewLine}");
            return;
        }
        ParserService parserService = new();
        var list = parserService.ParseTextFile(filename);
        DictionaryService dictionaryService = new();
        await dictionaryService.UpdateDictionary(list);
    }


    private async Task ClearDictionary(IEnumerable<string> message)
    {
        DictionaryService dictionaryService = new();
        await dictionaryService.ClearDictionary();
    }

    private async Task GetDictionary(IEnumerable<string> message)
    {
        string? prefix = message.FirstOrDefault();
        if (string.IsNullOrEmpty(prefix))
        {
            Console.WriteLine($"Failed to get prefix from command line arguments{Environment.NewLine}");
            return;
        }
        DictionaryService dictionaryService = new();
        string matches = await dictionaryService.GetDictionary(prefix);
        if(string.IsNullOrEmpty(matches))
        {
            Console.WriteLine($"No matches for {prefix}{Environment.NewLine}");
        }
        else
        {
            var words = matches.Split(Environment.NewLine);
            foreach (var word in words)
            {
                Console.WriteLine($"- {word}");
            }
            Console.WriteLine();
        }
    }
}
