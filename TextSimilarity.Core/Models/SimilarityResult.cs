namespace TextSimilarity.Core.Models;

public class SimilarityResult
{
    public string AlgorithmName { get; set; } = string.Empty;
    public double Score { get; set; }
    public string ScoreFormatted => $"{Score:P1}";
    public List<string> CommonTokens { get; set; } = new();
    public List<string> HighlightedPhrases { get; set; } = new();
}
