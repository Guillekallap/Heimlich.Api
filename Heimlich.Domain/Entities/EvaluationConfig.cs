namespace Heimlich.Domain.Entities
{
    public class EvaluationConfig
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public int MaxErrors { get; set; }
        public int MaxTime { get; set; }
        public string Name { get; set; } // Identificación desde mobile
        public bool IsDefault { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    }
}