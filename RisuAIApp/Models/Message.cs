#nullable enable
namespace RisuAIApp.Models;

using System.Text.Json.Serialization;

public struct Message {
    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }


    public const string ROLE_SYSTEM = "system";
    public const string ROLE_USER = "user";
    public const string ROLE_ASSISTANT = "assistant";
}
