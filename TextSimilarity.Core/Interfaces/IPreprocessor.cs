namespace TextSimilarity.Core.Interfaces;

public interface IPreprocessor
{
    string Normalize(string text);
    List<string> Tokenize(string text);
    List<string> RemoveStopWords(List<string> tokens);
    List<string> Process(string text);
}
