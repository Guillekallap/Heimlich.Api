using Heimlich.Domain.Enums;

namespace Heimlich.Application.DTOs
{
    public class CreatePracticeSessionDto
    {
        public DateTime CreationDate { get; set; }
        public int TrunkId { get; set; } // Obligatorio para simulación
        public int? GroupId { get; set; } // Solo para evaluación
        // Puedes agregar validaciones con DataAnnotations si lo prefieres
    }
}