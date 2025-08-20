using Heimlich.Domain.Enums;

namespace Heimlich.Application.DTOs
{
    public class CreatePracticeSessionDto
    {
        public PracticeTypeEnum PracticeType { get; set; }
        public DateTime CreationDate { get; set; }
    }
}