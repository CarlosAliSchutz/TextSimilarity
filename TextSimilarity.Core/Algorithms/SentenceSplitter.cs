using System.Text.RegularExpressions;

namespace TextSimilarity.Core.Algorithms;

public class SentenceSplitter
{
    // Divide o texto em sentenças por pontuação final
    public List<string> Split(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new List<string>();

        var sentences = Regex
            .Split(text.Trim(), @"(?<=[.!?])\s+")
            .Select(s => s.Trim())
            .Where(s => s.Length > 10) // ignora fragmentos muito curtos
            .ToList();

        return sentences;
    }
}
