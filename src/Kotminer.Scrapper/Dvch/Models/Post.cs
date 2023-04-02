using System.Text.Json.Serialization;

namespace Kotminer.Scrapper.Models;

public class Post
{
    [JsonPropertyName("comment")] public string Comment { get; set; }
    [JsonPropertyName("num")] public int Num { get; set; }
}