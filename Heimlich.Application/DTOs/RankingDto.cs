using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Heimlich.Application.DTOs
{
    public class PractitionerRankingDto
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        [JsonPropertyName("averageScore")]
        public double AverageScore { get; set; }

        [JsonPropertyName("evaluationCount")]
        public int EvaluationCount { get; set; }
    }

    public class GroupRankingDto
    {
        [JsonPropertyName("groupId")]
        public int GroupId { get; set; }

        [JsonPropertyName("groupName")]
        public string GroupName { get; set; }

        [JsonPropertyName("groupAverage")]
        public double GroupAverage { get; set; }

        [JsonPropertyName("practitioners")]
        public List<PractitionerRankingDto> Practitioners { get; set; }
    }
}