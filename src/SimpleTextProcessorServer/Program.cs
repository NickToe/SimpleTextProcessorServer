using CommandLine;
using SimpleTextProcessorServer.InputService;
using SimpleTextProcessorServer.Server;
using SimpleTextProcessorServer.Services;

namespace SimpleTextProcessorServer;

internal class Program
{
    static void Main(string[] args)
    {
        // use nuget CommandLineParser to parse command line arguments
        Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed(options =>
        {
            Console.WriteLine("Server started...");

            DictionaryService.DbPath = options.DbPath;

            // Start Server task in background that handles all incoming connections
            Task task1 = Task.Run(async () =>
            {
                TcpServer server = new();
                try
                {
                    server.Connect(options.Port);
                }
                catch(Exception ex)
                {
                    /* Most likely port is already in use, but handle all possible exceptions like this */
                    Console.WriteLine($"Main(): {ex.GetType()}: {ex.Message}");
                    Environment.Exit(1);
                }
                Console.WriteLine("Waiting for client connections...");
                await server.WaitForConnections();
            });

            // Start InputReader task in background that handles all console input for dictionary operations
            Task task2 = Task.Run(async () =>
            {
                InputReader inputReader = new();
                Console.WriteLine("Waiting for input...");
                await inputReader.WaitForInput();
            });

            Task.WaitAll(task1, task2);

            Console.WriteLine("Server shutting down...");
        });
    }
}