using TextSimilarity.Core.Algorithms;
using TextSimilarity.Core.Models;
using TextSimilarity.Core.Services;
using Xunit;

namespace TextSimilarity.Tests;

public class SimilarityTests
{
    private readonly Preprocessor _preprocessor = new();

    // ── Preprocessor ──────────────────────────────────────────

    [Fact]
    public void Normalize_RemovesAccentsAndLowercase()
    {
        var result = _preprocessor.Normalize("Análise de Conteúdo");
        Assert.Equal("analise de conteudo", result);
    }

    [Fact]
    public void Tokenize_SplitsCorrectly()
    {
        var tokens = _preprocessor.Tokenize("analise de conteudo textual");
        Assert.Contains("analise", tokens);
        Assert.Contains("conteudo", tokens);
    }

    [Fact]
    public void Process_RemovesStopWords()
    {
        var tokens = _preprocessor.Process("a análise de um texto");
        Assert.DoesNotContain("a", tokens);
        Assert.DoesNotContain("de", tokens);
        Assert.DoesNotContain("um", tokens);
        Assert.Contains("analise", tokens);
        Assert.Contains("texto", tokens);
    }

    // ── Jaccard ────────────────────────────────────────────────

    [Fact]
    public void Jaccard_IdenticalTexts_Returns1()
    {
        var jaccard = new JaccardSimilarity(_preprocessor);
        double score = jaccard.Calculate("texto igual para teste", "texto igual para teste");
        Assert.Equal(1.0, score, precision: 2);
    }

    [Fact]
    public void Jaccard_CompletelyDifferentTexts_Returns0()
    {
        var jaccard = new JaccardSimilarity(_preprocessor);
        double score = jaccard.Calculate("cachorro gato pássaro", "computador teclado monitor");
        Assert.Equal(0.0, score, precision: 2);
    }

    [Fact]
    public void Jaccard_PartialOverlap_ReturnsBetween0And1()
    {
        var jaccard = new JaccardSimilarity(_preprocessor);
        double score = jaccard.Calculate(
            "sistema de comparação textual eficiente",
            "sistema de análise textual automática");
        Assert.InRange(score, 0.1, 0.9);
    }

    // ── TF Cosseno ───────────────────────────────────────────

    [Fact]
    public void Tf_IdenticalTexts_ReturnsHigh()
    {
        var tf = new TfCosine(_preprocessor);
        double score = tf.Calculate(
            "processamento de linguagem natural",
            "processamento de linguagem natural");
        Assert.True(score > 0.90);
    }

    [Fact]
    public void Tf_SemanticallySimilar_ReturnsReasonableScore()
    {
        var tf = new TfCosine(_preprocessor);
        double score = tf.Calculate(
            "o sistema compara documentos textuais usando algoritmos",
            "algoritmos são usados para comparar documentos no sistema");
        Assert.True(score > 0.40);
    }

    // ── Levenshtein ────────────────────────────────────────────

    [Fact]
    public void Levenshtein_IdenticalStrings_Returns1()
    {
        var lev = new LevenshteinDistance(_preprocessor);
        double score = lev.CalculateNormalized("texto identico", "texto identico");
        Assert.Equal(1.0, score, precision: 2);
    }

    [Fact]
    public void Levenshtein_MinorTypo_ReturnsHigh()
    {
        var lev = new LevenshteinDistance(_preprocessor);
        double score = lev.CalculateNormalized("comparacao textual", "comparação textual");
        Assert.True(score > 0.85);
    }

    // ── ComparisonService ──────────────────────────────────────

    [Fact]
    public void ComparisonService_EmptyText_ThrowsException()
    {
        var service = new ComparisonService(_preprocessor);
        Assert.Throws<ArgumentException>(() =>
            service.Compare(new ComparisonRequest { Text1 = "", Text2 = "algum texto" }));
    }

    [Fact]
    public void ComparisonService_ConsolidatedScore_InRange()
    {
        var service = new ComparisonService(_preprocessor);
        var response = service.Compare(new ComparisonRequest
        {
            Text1 = "o plágio é identificado por algoritmos de similaridade textual",
            Text2 = "algoritmos de similaridade identificam ocorrências de plágio em textos"
        });

        Assert.InRange(response.ConsolidatedScore, 0.0, 1.0);
        Assert.Equal(3, response.Results.Count);
    }
}
