using TextSimilarity.Core.Interfaces;

namespace TextSimilarity.Core.Algorithms;

public class LevenshteinDistance : ISimilarityAlgorithm
{
    private readonly SentenceSplitter _splitter;
    private readonly IPreprocessor _preprocessor;

    public string Name => "Levenshtein";

    public LevenshteinDistance(IPreprocessor preprocessor)
    {
        _preprocessor = preprocessor;
        _splitter = new SentenceSplitter();
    }

    public double Calculate(string text1, string text2)
    {
        // Aplicado por sentença para evitar O(n²) em textos longos
        var sentences1 = _splitter.Split(text1);
        var sentences2 = _splitter.Split(text2);

        if (sentences1.Count == 0 || sentences2.Count == 0)
            return CalculateNormalized(text1, text2);

        // Para cada sentença do texto 1, encontra a sentença mais similar no texto 2
        double totalScore = 0.0;

        foreach (var s1 in sentences1)
        {
            double bestMatch = sentences2
                .Select(s2 => CalculateNormalized(s1, s2))
                .Max();

            totalScore += bestMatch;
        }

        return totalScore / sentences1.Count;
    }

    // Retorna score entre 0 e 1 (1 = idêntico)
    public double CalculateNormalized(string s1, string s2)
    {
        s1 = _preprocessor.Normalize(s1);
        s2 = _preprocessor.Normalize(s2);

        if (s1 == s2) return 1.0;
        if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2)) return 0.0;

        int distance = ComputeDistance(s1, s2);
        int maxLen = Math.Max(s1.Length, s2.Length);

        return 1.0 - ((double)distance / maxLen);
    }

    private static int ComputeDistance(string s1, string s2)
    {
        // Limita o tamanho para performance (máximo 500 chars por sentença)
        if (s1.Length > 500) s1 = s1[..500];
        if (s2.Length > 500) s2 = s2[..500];

        int m = s1.Length;
        int n = s2.Length;

        // Programação dinâmica — matriz m x n
        var dp = new int[m + 1, n + 1];

        for (int i = 0; i <= m; i++) dp[i, 0] = i;
        for (int j = 0; j <= n; j++) dp[0, j] = j;

        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {
                int cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                dp[i, j] = Math.Min(
                    Math.Min(dp[i - 1, j] + 1,    // deleção
                             dp[i, j - 1] + 1),    // inserção
                             dp[i - 1, j - 1] + cost); // substituição
            }
        }

        return dp[m, n];
    }
}
