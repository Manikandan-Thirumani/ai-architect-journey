using UglyToad.PdfPig;

namespace AiLearningApi.Services;

public class PdfKnowledgeService
{
    public string ExtractTextFromPdf(string path)
    {
        using var document = PdfDocument.Open(path);

        var text = "";

        foreach (var page in document.GetPages())
        {
            text += page.Text + "\n";
        }

        return text;
    }

    public string SearchRelevantPolicy(
        string question)
    {
        var pdfText = ExtractTextFromPdf(
            "Documents/banking-policy.pdf");

        // VERY SIMPLE retrieval

        var lines = pdfText.Split('\n');

        foreach (var line in lines)
        {
            if (question.ToLower()
                .Contains("loan")
                && line.ToLower().Contains("loan"))
            {
                return line;
            }

            if (question.ToLower()
                .Contains("age")
                && line.ToLower().Contains("age"))
            {
                return line;
            }
        }

        return """
        No relevant banking policy found.
        """;
    }
}