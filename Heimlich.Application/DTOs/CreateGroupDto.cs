namespace Heimlich.Application.DTOs
{
    public class CreateGroupDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> PractitionerIds { get; set; }
    }
}