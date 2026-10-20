using TextSimilarity.Core.Interfaces;

namespace TextSimilarity.Core.Algorithms;

public class TfCosine : ISimilarityAlgorithm
{
    private readonly IPreprocessor _preprocessor;

    public string Name => "TF + Cosseno";

    public TfCosine(IPreprocessor preprocessor)
    {
        _preprocessor = preprocessor;
    }

    public double Calculate(string text1, string text2)
    {
        var tokens1 = _preprocessor.Process(text1);
        var tokens2 = _preprocessor.Process(text2);

        if (tokens1.Count == 0 || tokens2.Count == 0)
            return 0.0;

        var vocabulary = tokens1.Union(tokens2).Distinct().ToList();

        var tf1 = ComputeTf(tokens1);
        var tf2 = ComputeTf(tokens2);

        var vector1 = vocabulary.Select(t => tf1.GetValueOrDefault(t, 0.0)).ToArray();
        var vector2 = vocabulary.Select(t => tf2.GetValueOrDefault(t, 0.0)).ToArray();

        return CosineSimilarity(vector1, vector2);
    }

    private static Dictionary<string, double> ComputeTf(List<string> tokens)
    {
        var tf = new Dictionary<string, double>();
        foreach (var token in tokens)
            tf[token] = tf.GetValueOrDefault(token) + 1;

        // Normaliza pela contagem total
        foreach (var key in tf.Keys.ToList())
            tf[key] /= tokens.Count;

        return tf;
    }

    private static double CosineSimilarity(double[] v1, double[] v2)
    {
        double dotProduct = v1.Zip(v2, (a, b) => a * b).Sum();
        double norm1 = Math.Sqrt(v1.Sum(x => x * x));
        double norm2 = Math.Sqrt(v2.Sum(x => x * x));

        if (norm1 == 0 || norm2 == 0)
            return 0.0;

        return dotProduct / (norm1 * norm2);
    }
}
