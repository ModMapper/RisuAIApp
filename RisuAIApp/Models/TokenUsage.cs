namespace RisuAIApp.Models;

using System.Text.Json.Serialization;

public class TokenUsage {
    [JsonPropertyName("prompt_tokens")]
    public int Prompt { get; set; }

    [JsonPropertyName("completion_tokens")]
    public int Completion { get; set; }

    [JsonPropertyName("total_tokens")]
    public int Total { get; set; }
    
}
