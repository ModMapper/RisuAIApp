#nullable enable
namespace RisuAIApp.Models;

using System.Text.Json.Serialization;

public class Response {
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("object")]
    public string? Type { get; set; }

    [JsonPropertyName("created")]
    public long Created { get; set; }

    [JsonPropertyName("choices")]
    public IEnumerable<MessageResult>? Choices { get; set; }

    [JsonPropertyName("usage")]
    public TokenUsage? Usage { get; set; }
}
