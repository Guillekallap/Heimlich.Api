namespace Heimlich.Application.DTOs
{
    public class CreateGroupDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> PractitionerIds { get; set; }
        public int? EvaluationConfigId { get; set; } // opcional: vincular config existente, si null se usa Default
    }
}