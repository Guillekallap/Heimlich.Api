public class EvaluationParametersDto
{
    public List<SensorIntervalDto> SensorIntervals { get; set; }
    public int MaxErrors { get; set; }
    public int MaxTime { get; set; }
    public string Name { get; set; }
}