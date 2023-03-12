using System.Text;
using Microsoft.EntityFrameworkCore;
using SimpleTextProcessorServer.Data;

namespace SimpleTextProcessorServer.Services;

public class DictionaryService
{
    private readonly ApplicationDbContext _dbContext;
    public static string DbPath { get; set; } = "defaultTextProcessor.db";

    public DictionaryService()
    {
        _dbContext = new($"Server=(localdb)\\mssqllocaldb;Database={DbPath};");
        _dbContext.Database.EnsureCreated();
    }


    public async Task CreateDictionary(IEnumerable<WordDictionary> words)
    {
        if (!words.Any()) return;

        if (await _dbContext.WordDictionary.AnyAsync())
        {
            Console.WriteLine("Can't create dictionary: it is not empty!");
            Console.WriteLine("Please use 'update <filename>'");
            Console.WriteLine($"Or 'clear' to clear dictionary{Environment.NewLine}");
            return;
        }

        await _dbContext.WordDictionary.AddRangeAsync(words);

        int saved = await _dbContext.SaveChangesAsync();
        Console.WriteLine($"Saved {saved} words{Environment.NewLine}");
    }


    public async Task UpdateDictionary(IEnumerable<WordDictionary> words)
    {
        if (!words.Any()) return;

        foreach (var tempWord in words)
        {
            if (!await _dbContext.WordDictionary.AnyAsync(word => word.Word == tempWord.Word))
            {
                await _dbContext.WordDictionary.AddAsync(tempWord);
            }
        }

        int saved = await _dbContext.SaveChangesAsync();
        Console.WriteLine($"Saved {saved} words{Environment.NewLine}");
    }


    public async Task ClearDictionary()
    {
        using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
        {
            await _dbContext.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE {nameof(WordDictionary)}");
            await dbContextTransaction.CommitAsync();
        }
        Console.WriteLine($"Table cleared{Environment.NewLine}");
    }


    public async Task<string> GetDictionary(string userInput)
    {
        var wordsList = await _dbContext.WordDictionary
            .Where(word => word.Word.StartsWith(userInput))
            .OrderByDescending(word => word.Counter)
            .ThenBy(word => word.Word)
            .Take(5)
            .AsNoTracking()
            .ToListAsync();

        foreach (var word in wordsList)
        {
            Console.WriteLine($"Word: {word.Word}; Counter: {word.Counter}");
        }
        Console.WriteLine();

        StringBuilder stringBuilder = new StringBuilder(String.Empty);

        if (wordsList.Count > 0)
        {
            // Format found words as "word + newline"
            wordsList.ForEach(word => stringBuilder.AppendLine($"{word.Word}"));

            // Remove trailing newline
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
        }

        return stringBuilder.ToString();
    }
}
