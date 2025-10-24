using Heimlich.Domain.Enums;

namespace Heimlich.Domain.Entities
{
    public class EvaluationConfig
    {
        public int Id { get; set; }
        public int? GroupId { get; set; } // grupo opcional (config reutilizable)
        public Group? Group { get; set; }
        public int MaxErrors { get; set; }
        public int MaxTime { get; set; }
        public int MaxTimeInterval { get; set; } // Lapso en segundos entre evaluaciones automáticas
        public string Name { get; set; } // Identificación desde mobile (único a nivel sistema)
        public bool IsDefault { get; set; }
        public EvaluationConfigStatusEnum Status { get; set; } = EvaluationConfigStatusEnum.Active;
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public ICollection<EvaluationConfigGroup> EvaluationConfigGroups { get; set; } = new List<EvaluationConfigGroup>();
    }
}