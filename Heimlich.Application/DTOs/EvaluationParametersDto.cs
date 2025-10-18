public class EvaluationParametersDto
{
    public List<SensorIntervalDto> SensorIntervals { get; set; }
    public int MaxErrors { get; set; }
    public int MaxTime { get; set; }
    public int MaxTimeInterval { get; set; } // Lapso en segundos entre evaluaciones automáticas
    public string Name { get; set; }
}