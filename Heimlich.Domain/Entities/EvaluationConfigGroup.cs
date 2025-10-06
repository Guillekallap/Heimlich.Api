namespace Heimlich.Domain.Entities
{
    public class EvaluationConfigGroup
    {
        public int EvaluationConfigId { get; set; }
        public EvaluationConfig EvaluationConfig { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}