using TextSimilarity.Core.Algorithms;
using TextSimilarity.Core.Helpers;
using TextSimilarity.Core.Models;
using TextSimilarity.Core.Services;

var preprocessor = new Preprocessor();
var service = new ComparisonService(preprocessor);

Console.OutputEncoding = System.Text.Encoding.UTF8;
PrintHeader();

while (true)
{
    Console.WriteLine(
        "┌─────────────────────────────────────────┐\n" +
        "│  1. Comparar dois textos                │\n" +
        "│  2. Comparar dois arquivos .txt ou pdf  │\n" +
        "│  0. Sair                                │\n" +
        "└─────────────────────────────────────────┘\n");
    Console.Write("\nEscolha: ");

    var option = Console.ReadLine()?.Trim();

    switch (option)
    {
        case "1": CompareTexts(service); break;
        case "2": CompareFiles(service); break;
        case "0":
            Console.WriteLine("\nEncerrando...");
            return;
        default:
            Console.WriteLine("Opção inválida.");
            break;
    }
}

static void CompareTexts(ComparisonService service)
{
    Console.WriteLine("\n── Texto 1 (pressione Enter para finalizar) ──");
    string text1 = ReadMultilineInput();

    Console.WriteLine("\n── Texto 2 (pressione Enter para finalizar) ──");
    string text2 = ReadMultilineInput();

    RunComparison(service, text1, text2);
}

static void CompareFiles(ComparisonService service)
{
    Console.Write("\nCaminho do arquivo 1: ");
    string path1 = Console.ReadLine()?.Trim() ?? "";

    Console.Write("Caminho do arquivo 2: ");
    string path2 = Console.ReadLine()?.Trim() ?? "";

    try
    {
        string text1 = ReadFile(path1);
        string text2 = ReadFile(path2);
        RunComparison(service, text1, text2);
    }
    catch (FileNotFoundException)
    {
        Console.WriteLine("\n[ERRO] Arquivo não encontrado. Verifique o caminho e tente novamente.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n[ERRO] {ex.Message}");
    }
}

static void RunComparison(ComparisonService service, string text1, string text2)
{
    Console.WriteLine("\nProcessando...\n");
    try
    {
        var response = service.Compare(new ComparisonRequest { Text1 = text1, Text2 = text2 });
        PrintResults(response);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERRO] {ex.Message}");
    }
}

static void PrintResults(ComparisonResponse response)
{
    Console.WriteLine("╔══════════════════════════════════════════╗");
    Console.WriteLine("║         RESULTADO DA COMPARAÇÃO          ║");
    Console.WriteLine("╠══════════════════════════════════════════╣");

    foreach (var r in response.Results)
        Console.WriteLine($"║  {r.AlgorithmName,-20} {r.ScoreFormatted,8}           ║");

    Console.WriteLine($"╠══════════════════════════════════════════╣");
    Console.WriteLine($"║  Score Consolidado:   {response.ConsolidatedScoreFormatted,8}           ║");
    Console.WriteLine($"╚══════════════════════════════════════════╝");

    Console.WriteLine($"\n Interpretação: {response.Interpretation}");

    var jaccardTokens = response.Results.FirstOrDefault(r => r.AlgorithmName == "Jaccard")?.CommonTokens;
    if (jaccardTokens?.Any() == true)
        Console.WriteLine($"\n Tokens em comum ({jaccardTokens.Count}): {string.Join(", ", jaccardTokens.Take(15))}\n");

    if (response.CommonPhrases.Any())
    {
        Console.WriteLine("\n Trechos similares identificados:\n");
        foreach (var phrase in response.CommonPhrases)
            Console.WriteLine($"*   {phrase}");
        Console.WriteLine("\n");
    }
}

static string ReadMultilineInput()
{
    var lines = new List<string>();
    string? line;
    while (!string.IsNullOrEmpty(line = Console.ReadLine()))
        lines.Add(line);
    return string.Join(" ", lines);
}

static string ReadFile(string path)
{
    var extension = Path.GetExtension(path).ToLower();

    return extension switch
    {
        ".txt" => File.ReadAllText(path, System.Text.Encoding.UTF8),
        ".pdf" => PdfTextExtractor.ExtractText(path),
        _ => throw new Exception("Formato não suportado.")
    };
}

static void PrintHeader()
{
    Console.Clear();
    Console.WriteLine("╔══════════════════════════════════════════════╗");
    Console.WriteLine("║     TEXT SIMILARITY — Sistema de Comparação  ║");
    Console.WriteLine("║            TF Jaccard Levenshtein            ║");
    Console.WriteLine("╚══════════════════════════════════════════════╝");
}
