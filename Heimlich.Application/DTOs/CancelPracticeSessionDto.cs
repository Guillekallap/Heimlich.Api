namespace Heimlich.Application.DTOs
{
    public class CancelPracticeSessionDto
    {
        public int SessionId { get; set; }
        public TimeSpan TotalTime { get; set; }
        public int ErrorCount { get; set; }
        public string Status { get; set; }
    }
}