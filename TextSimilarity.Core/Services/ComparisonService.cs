using TextSimilarity.Core.Algorithms;
using TextSimilarity.Core.Interfaces;
using TextSimilarity.Core.Models;

namespace TextSimilarity.Core.Services;

public class ComparisonService
{
    private readonly List<ISimilarityAlgorithm> _algorithms;
    private readonly JaccardSimilarity _jaccard;
    private readonly HighlightService _highlightService;

    public ComparisonService(IPreprocessor preprocessor)
    {
        _jaccard = new JaccardSimilarity(preprocessor);

        _algorithms = new List<ISimilarityAlgorithm>
        {
            new TfCosine(preprocessor),
            _jaccard,
            new LevenshteinDistance(preprocessor)
        };

        _highlightService = new HighlightService(preprocessor);
    }

    public ComparisonResponse Compare(ComparisonRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text1) || string.IsNullOrWhiteSpace(request.Text2))
            throw new ArgumentException("Os dois textos devem ser fornecidos.");

        var results = _algorithms.Select(algo => new SimilarityResult
        {
            AlgorithmName = algo.Name,
            Score = Math.Round(algo.Calculate(request.Text1, request.Text2), 4),
            CommonTokens = algo is JaccardSimilarity j
                ? j.GetCommonTokens(request.Text1, request.Text2)
                : new List<string>()
        }).ToList();

        double consolidated = results
            .Sum(r => r.Score * r.AlgorithmName switch
            {
                "TF + Cosseno" => 0.50,
                "Jaccard" => 0.30,
                "Levenshtein" => 0.20,
                _ => 0.0
            });

        var similarPhrases = _highlightService
            .FindSimilarPhrases(request.Text1, request.Text2)
            .Take(5)
            .Select(m => $"[{m.Score:P0}] \"{m.Sentence1}\"")
            .ToList();

        return new ComparisonResponse
        {
            Results = results,
            ConsolidatedScore = Math.Round(consolidated, 4),
            CommonPhrases = similarPhrases
        };
    }
}
