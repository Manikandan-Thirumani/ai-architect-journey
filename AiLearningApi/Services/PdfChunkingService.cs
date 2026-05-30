using UglyToad.PdfPig;
using AiLearningApi.Models;

namespace AiLearningApi.Services;

public class PdfChunkingService
{
    public List<DocumentChunk> LoadChunks()
    {
        var pdfText =
            ExtractTextFromPdf(
                "Documents/banking-policy.pdf");

        return SplitIntoChunks(pdfText);
    }

    private string ExtractTextFromPdf(
        string path)
    {
        using var document =
            PdfDocument.Open(path);

        var text = "";

        foreach (var page in document.GetPages())
        {
            text += page.Text + "\n";
        }

        return text;
    }

    private List<DocumentChunk> SplitIntoChunks(
        string text)
    {
        var chapters =
            text.Split(
                "Chapter ",
                StringSplitOptions.RemoveEmptyEntries);

        var chunks =
            new List<DocumentChunk>();

        int id = 1;

        foreach (var chapter in chapters)
        {
            chunks.Add(new DocumentChunk
            {
                Id = id++,
                Content = "Chapter " + chapter
            });
        }

        return chunks;
    }
}