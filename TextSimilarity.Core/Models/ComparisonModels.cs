namespace TextSimilarity.Core.Models;

public class ComparisonRequest
{
    public string Text1 { get; set; } = string.Empty;
    public string Text2 { get; set; } = string.Empty;
}

public class ComparisonResponse
{
    public List<SimilarityResult> Results { get; set; } = new();
    public double ConsolidatedScore { get; set; }
    public string ConsolidatedScoreFormatted => $"{ConsolidatedScore:P1}";
    public string Interpretation => ConsolidatedScore switch
    {
        >= 0.85 => "Textos muito similares — alto risco de duplicidade ou plágio.",
        >= 0.60 => "Similaridade significativa — revisar trechos em comum.",
        >= 0.30 => "Similaridade moderada — possível reuso parcial de conteúdo.",
        _       => "Textos distintos — baixa similaridade identificada."
    };
    public List<string> CommonPhrases { get; set; } = new();
}
