using Heimlich.Domain.Entities;

namespace Heimlich.Application.DTOs
{
    public class PracticeSessionDto
    {
        public int Id { get; set; }
        public PracticeType PracticeType { get; set; }
        public DateTime CreationDate { get; set; }
    }
}