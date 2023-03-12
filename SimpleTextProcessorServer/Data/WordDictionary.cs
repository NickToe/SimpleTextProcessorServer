namespace SimpleTextProcessorServer.Data;

public sealed class WordDictionary
{
    public int Id { get; set; }
    public string Word { get; set; } = null!;
    public int Counter { get; set; }
}
