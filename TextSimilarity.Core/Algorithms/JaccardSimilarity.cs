using TextSimilarity.Core.Interfaces;

namespace TextSimilarity.Core.Algorithms;

public class JaccardSimilarity : ISimilarityAlgorithm
{
    private readonly IPreprocessor _preprocessor;

    public string Name => "Jaccard";

    public JaccardSimilarity(IPreprocessor preprocessor)
    {
        _preprocessor = preprocessor;
    }

    public double Calculate(string text1, string text2)
    {
        var tokens1 = new HashSet<string>(_preprocessor.Process(text1));
        var tokens2 = new HashSet<string>(_preprocessor.Process(text2));

        if (tokens1.Count == 0 && tokens2.Count == 0)
            return 1.0;

        if (tokens1.Count == 0 || tokens2.Count == 0)
            return 0.0;

        // |A ∩ B| / |A ∪ B|
        var intersection = tokens1.Intersect(tokens2).Count();
        var union = tokens1.Union(tokens2).Count();

        return (double)intersection / union;
    }

    // Retorna os tokens em comum (útil para highlight)
    public List<string> GetCommonTokens(string text1, string text2)
    {
        var tokens1 = new HashSet<string>(_preprocessor.Process(text1));
        var tokens2 = new HashSet<string>(_preprocessor.Process(text2));
        return tokens1.Intersect(tokens2).OrderBy(t => t).ToList();
    }
}
