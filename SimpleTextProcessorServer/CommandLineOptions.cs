using CommandLine;

namespace SimpleTextProcessorServer;

public sealed record CommandLineOptions
{
    [Option("database", Required = true, HelpText = "Path to dictionary database")]
    public string DbPath { get; set; } = null!;

    [Option("port", Required = true, HelpText = "Port number for server")]
    public int Port { get; set; }
}
