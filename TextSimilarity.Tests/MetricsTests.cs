using TextSimilarity.Core.Algorithms;
using TextSimilarity.Core.Models;
using TextSimilarity.Core.Services;
using Xunit;
using Xunit.Abstractions;

namespace TextSimilarity.Tests;

/// <summary>
/// Avalia o sistema usando Precisão, Revocação e F1-Score.
/// 
/// Definição para este contexto:
///   - Positivo: par de textos classificado como similar (score >= threshold)
///   - Negativo: par classificado como distinto
///   - Verdadeiro Positivo (VP): similar e realmente é
///   - Falso Positivo (FP): similar mas não deveria ser
///   - Falso Negativo (FN): não similar mas deveria ser
/// </summary>
public class MetricsTests
{
    private readonly ITestOutputHelper _output;
    private readonly ComparisonService _service;
    private const double Threshold = 0.50;

    public MetricsTests(ITestOutputHelper output)
    {
        _output = output;
        _service = new ComparisonService(new Preprocessor());
    }

    private static readonly List<(string Text1, string Text2, bool ExpectedSimilar)> TestCases = new()
    {
        // Verdadeiros Positivos esperados (textos realmente similares)
        ("O sistema compara documentos textuais.",
         "O sistema realiza comparação de documentos textuais.", true),

        ("Algoritmos de similaridade identificam plágio.",
         "Plágio é identificado por algoritmos de similaridade textual.", true),

        ("Processamento de linguagem natural permite análise automática de textos.",
         "A análise automática de textos é feita com processamento de linguagem natural.", true),

        ("TF pondera a relevância dos termos em documentos.",
         "A relevância dos termos nos documentos é ponderada pelo TF.", true),

        // Verdadeiros Negativos esperados (textos realmente distintos)
        ("O gato dormiu sobre o telhado durante a tarde.",
         "O sistema de banco de dados armazena informações estruturadas.", false),

        ("Futebol é o esporte mais popular do Brasil.",
         "A inteligência artificial transforma a indústria tecnológica.", false),

        ("Receita de bolo de chocolate com cobertura.",
         "Protocolo TCP/IP e camadas de rede OSI.", false),

        ("O sol nasce no leste e se põe no oeste.",
         "Algoritmos de ordenação: quicksort, mergesort e heapsort.", false),
    };

    [Fact]
    public void EvaluateMetrics_PrintPrecisionRecallF1()
    {
        int vp = 0, fp = 0, fn = 0, vn = 0;

        foreach (var (text1, text2, expected) in TestCases)
        {
            var response = _service.Compare(new ComparisonRequest { Text1 = text1, Text2 = text2 });
            bool predicted = response.ConsolidatedScore >= Threshold;

            _output.WriteLine($"Score: {response.ConsolidatedScore:P1} | Esperado: {(expected ? "Similar" : "Distinto")} | Predito: {(predicted ? "Similar" : "Distinto")}");

            if (predicted && expected)  vp++;
            if (predicted && !expected) fp++;
            if (!predicted && expected) fn++;
            if (!predicted && !expected) vn++;
        }

        double precision = vp + fp > 0 ? (double)vp / (vp + fp) : 0;
        double recall    = vp + fn > 0 ? (double)vp / (vp + fn) : 0;
        double f1        = precision + recall > 0
            ? 2 * precision * recall / (precision + recall)
            : 0;

        _output.WriteLine("");
        _output.WriteLine($"VP={vp} | FP={fp} | FN={fn} | VN={vn}");
        _output.WriteLine($"Precisão : {precision:P1}");
        _output.WriteLine($"Revocação: {recall:P1}");
        _output.WriteLine($"F1-Score : {f1:P1}");

        // Assertions mínimas esperadas para o TCC
        Assert.True(precision >= 0.60, $"Precisão abaixo do mínimo: {precision:P1}");
        Assert.True(recall    >= 0.60, $"Revocação abaixo do mínimo: {recall:P1}");
        Assert.True(f1        >= 0.60, $"F1-Score abaixo do mínimo: {f1:P1}");
    }
}
