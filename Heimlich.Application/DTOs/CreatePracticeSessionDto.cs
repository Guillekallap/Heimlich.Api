using Heimlich.Domain.Entities;

namespace Heimlich.Application.DTOs
{
    public class CreatePracticeSessionDto
    {
        public PracticeType PracticeType { get; set; }
        public DateTime CreationDate { get; set; }
    }
}