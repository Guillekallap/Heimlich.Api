namespace Heimlich.Application.DTOs
{
    public class MeasurementDto
    {
        public DateTime Time { get; set; }
        public string MetricType { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public string Source { get; set; }
    }
}