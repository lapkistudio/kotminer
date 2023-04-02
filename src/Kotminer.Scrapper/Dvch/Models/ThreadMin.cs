using System.Text.Json.Serialization;

namespace Kotminer.Scrapper.Models;

public class ThreadMin
{
    [JsonPropertyName("num")] public int Num { get; set; }
}