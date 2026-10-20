using UglyToad.PdfPig;

namespace TextSimilarity.Core.Helpers
{
    public static class PdfTextExtractor
    {
        public static string ExtractText(string path)
        {
            using var document = PdfDocument.Open(path);

            var text = new System.Text.StringBuilder();

            foreach (var page in document.GetPages())
                text.AppendLine(page.Text);

            return text.ToString();
        }
    }
}