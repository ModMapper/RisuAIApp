#nullable enable
namespace RisuAIApp.Models;

using System.Text.Json.Serialization;

public struct MessageResult {
    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("message")]
    public Message? Message { get; set; }

    [JsonPropertyName("finish_reason")]
    public string? Reason { get; set; }
}
