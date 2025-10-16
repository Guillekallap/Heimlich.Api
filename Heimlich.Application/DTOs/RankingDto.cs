namespace Heimlich.Application.DTOs
{
    public class PractitionerRankingDto
    {
        public string UserId { get; set; }
        public double AverageScore { get; set; }
        public int EvaluationCount { get; set; }
    }

    public class GroupRankingDto
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public double GroupAverage { get; set; }
        public List<PractitionerRankingDto> Practitioners { get; set; }
    }
}