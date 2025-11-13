using System.Text.Json.Serialization;

public class EvaluationParametersDto
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

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

    [JsonPropertyName("isDefault")]
    public bool? IsDefault { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}