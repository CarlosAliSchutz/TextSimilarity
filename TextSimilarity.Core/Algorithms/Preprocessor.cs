using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using TextSimilarity.Core.Interfaces;

namespace TextSimilarity.Core.Algorithms;

public class Preprocessor : IPreprocessor
{
    // Stop words em português e inglês
    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        // Português
        "a", "ao", "aos", "aquela", "aquelas", "aquele", "aqueles", "aquilo", "as",
        "até", "com", "como", "da", "das", "de", "dela", "delas", "dele", "deles",
        "depois", "do", "dos", "e", "ela", "elas", "ele", "eles", "em", "entre",
        "era", "essa", "essas", "esse", "esses", "esta", "estas", "este", "estes",
        "eu", "foi", "foram", "há", "isso", "isto", "já", "lhe", "lhes", "mais",
        "mas", "me", "mesmo", "meu", "minha", "muito", "na", "nas", "nem", "no",
        "nos", "nós", "num", "numa", "o", "os", "ou", "para", "pela", "pelas",
        "pelo", "pelos", "por", "qual", "quando", "que", "se", "sem", "ser",
        "seu", "seus", "só", "sua", "suas", "também", "te", "tem", "então",
        "tu", "tua", "um", "uma", "umas", "uns", "você", "vocês",
        // Inglês
        "a", "an", "the", "and", "or", "but", "in", "on", "at", "to", "for",
        "of", "with", "by", "from", "is", "are", "was", "were", "be", "been",
        "have", "has", "had", "do", "does", "did", "not", "this", "that", "it"
    };

    public string Normalize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Converte para minúsculas
        text = text.ToLowerInvariant();

        // Remove acentuação
        text = RemoveAccents(text);

        // Remove pontuação e caracteres especiais, mantém espaços
        text = Regex.Replace(text, @"[^a-z0-9\s]", " ");

        // Colapsa múltiplos espaços
        text = Regex.Replace(text, @"\s+", " ").Trim();

        return text;
    }

    public List<string> Tokenize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new List<string>();

        return text
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(t => t.Length > 1) // remove tokens de 1 caractere
            .ToList();
    }

    public List<string> RemoveStopWords(List<string> tokens)
    {
        return tokens
            .Where(t => !StopWords.Contains(t))
            .ToList();
    }

    public List<string> Process(string text)
    {
        var normalized = Normalize(text);
        var tokens = Tokenize(normalized);
        return RemoveStopWords(tokens);
    }

    private static string RemoveAccents(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
