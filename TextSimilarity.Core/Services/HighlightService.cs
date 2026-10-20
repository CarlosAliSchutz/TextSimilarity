using TextSimilarity.Core.Algorithms;
using TextSimilarity.Core.Interfaces;

namespace TextSimilarity.Core.Services;

public class HighlightService
{
    private readonly SentenceSplitter _splitter;
    private readonly IPreprocessor _preprocessor;
    private const double SimilarityThreshold = 0.70;

    public HighlightService(IPreprocessor preprocessor)
    {
        _preprocessor = preprocessor;
        _splitter = new SentenceSplitter();
    }

    // Retorna pares de sentenças similares entre os dois textos
    public List<(string Sentence1, string Sentence2, double Score)> FindSimilarPhrases(
        string text1, string text2)
    {
        var sentences1 = _splitter.Split(text1);
        var sentences2 = _splitter.Split(text2);
        var lev = new LevenshteinDistance(_preprocessor);
        var matches = new List<(string, string, double)>();

        foreach (var s1 in sentences1)
        {
            foreach (var s2 in sentences2)
            {
                double score = lev.CalculateNormalized(s1, s2);
                if (score >= SimilarityThreshold)
                    matches.Add((s1, s2, score));
            }
        }

        return matches
            .OrderByDescending(m => m.Item3)
            .ToList();
    }

    // Versão simplificada: apenas retorna as sentenças do texto1 que têm match
    public List<string> GetHighlightedSentences(string text1, string text2)
    {
        return FindSimilarPhrases(text1, text2)
            .Select(m => m.Sentence1)
            .Distinct()
            .ToList();
    }
}
