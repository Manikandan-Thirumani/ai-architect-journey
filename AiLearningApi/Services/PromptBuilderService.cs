public class PromptBuilderService
{
    public string BuildPrompt(string context, string question)
    {
        return $@"
You are an enterprise banking AI assistant.

RULES:
- Use ONLY the context below
- Do NOT hallucinate
- If not found, answer: 'Not found in policy'

Return ONLY valid JSON in this format:

{{
  ""answer"": ""..."",
  ""confidence"": 0.0 to 1.0,
  ""reasoning"": ""short explanation""
}}

CONTEXT:
{context}

QUESTION:
{question}
";
    }
}