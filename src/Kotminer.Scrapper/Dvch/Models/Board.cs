using System.Text.Json.Serialization;

namespace Kotminer.Scrapper.Dvch.Models;

public class Board
{
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("max_pages")] public int Pages { get; set; }
}