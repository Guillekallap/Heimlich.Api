using Heimlich.Domain.Enums;

namespace Heimlich.Application.DTOs
{
    public class PracticeSessionDto
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string PractitionerId { get; set; }
        public PracticeTypeEnum PracticeType { get; set; }
        public int? TrunkId { get; set; }
        public int? GroupId { get; set; }
    }
}