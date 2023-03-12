using System.Text.RegularExpressions;
using SimpleTextProcessorServer.Data;

namespace SimpleTextProcessorServer.Services;

public class ParserService
{
    public IEnumerable<WordDictionary> ParseTextFile(string filename)
    {
        if (!File.Exists(filename))
        {
            Console.WriteLine($"File '{filename}' doesn't exist{Environment.NewLine}");
            return Enumerable.Empty<WordDictionary>();
        }

        Dictionary<string, int> wordCounter = new();
        using (StreamReader stream = new(filename))
        {
            string? line = string.Empty;
            List<string> tempStrs = new List<string>();
            // Read file line by line until there's nothing to read
            while ((line = stream.ReadLine()) != null)
            {
                // Replace newline with whitespace and split words by whitespaces
                tempStrs = line.Replace(Environment.NewLine, " ").Split(' ').ToList();

                /* Remove all non-letter symbols at the beginning and at the end (for example, punctuation marks)
                 * but keep them at the middle, so, for example, words like "They're" or "semi-final" are considered as one.
                 * Also make words lowercase, so "Hello" and "hello" wouldn't be considered as different words.
                 */
                for (int i = 0; i < tempStrs.Count; i++)
                {
                    tempStrs[i] = Regex.Replace(tempStrs[i], "(^[^\\w]+)|([^\\w]+$)", "").ToLower();
                }

                // Only keep words that has at least 3 and at most 15 characters
                tempStrs = tempStrs.Where(s => s.Length >= 3 && s.Length <= 15).ToList();

                // If word is not in the collection, add it with counter = 1, otherwise increment counter
                foreach (string str in tempStrs)
                {
                    if (wordCounter.ContainsKey(str))
                    {
                        wordCounter[str]++;
                    }
                    else
                    {
                        wordCounter.Add(str, 1);
                    }
                }
            }
        }

        // Only keep words that appeared at least 3 times in the text
        var list = wordCounter.AsParallel().Where(word => word.Value >= 3).Select(word => new WordDictionary() { Word = word.Key, Counter = word.Value }).ToList();

        return list;
    }
}
