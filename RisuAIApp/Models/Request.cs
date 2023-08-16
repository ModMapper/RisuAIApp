#nullable enable
namespace RisuAIApp.Models;

using System.Text.Json.Serialization;

public class Request {
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("messages")]
    public Message[]? Messages { get; set; }

    [JsonPropertyName("temperature")]
    public decimal Temperature { get; set; } = 1;

    [JsonPropertyName("top_p")]
    public decimal Top_P { get; set; } = 1;

    [JsonPropertyName("n")]
    public int Count { get; set; } = 1;

    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = false;

    [JsonPropertyName("stop")]
    public string[]? StopId { get; set; }

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 0;

    [JsonPropertyName("presence_penalty")]
    public decimal PresencePenalty { get; set; } = 0;

    [JsonPropertyName("frequency_penalty")]
    public decimal FrequencyPenalty { get; set; } = 0;

    [JsonPropertyName("logit_bias")]
    public Dictionary<string, int> LogitBias { get; set; } = new();

    [JsonPropertyName("user")]
    public string? User { get; set; }
}
