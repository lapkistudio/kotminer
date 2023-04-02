using System.Text.Json.Serialization;

namespace Kotminer.Scrapper.Dvch.Models;

public class Thread
{
    [JsonPropertyName("comment")] public string Comment { get; set; }
    [JsonPropertyName("posts")] public Post?[] Posts { get; set; }
    [JsonPropertyName("num")] public int Num { get; set; }
}