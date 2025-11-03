using System.Text.Json.Serialization;

public class EvaluationParametersDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("maxErrors")]
    public int MaxErrors { get; set; }

    [JsonPropertyName("maxSuccess")]
    public int MaxSuccess { get; set; }

    [JsonPropertyName("maxTime")]
    public int MaxTime { get; set; }

    [JsonPropertyName("maxTimeInterval")]
    public int MaxTimeInterval { get; set; } // Lapso en segundos entre evaluaciones automáticas
}