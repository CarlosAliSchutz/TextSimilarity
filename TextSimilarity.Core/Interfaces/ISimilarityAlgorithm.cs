namespace TextSimilarity.Core.Interfaces;

public interface ISimilarityAlgorithm
{
    string Name { get; }
    double Calculate(string text1, string text2);
}
