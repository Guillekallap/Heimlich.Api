using System;
using Heimlich.Domain.Enums;

namespace Heimlich.Domain.Entities
{
    public class Measurement
    {
        public int Id { get; set; }
        public int? SimulationId { get; set; }
        public Simulation Simulation { get; set; }
        public int? EvaluationId { get; set; }
        public Evaluation Evaluation { get; set; }
        public DateTime Time { get; set; }
        public long? ElapsedMs { get; set; }

        // Nuevos campos para cada tipo de métrica
        public string ForceValue { get; set; }
        public bool ForceIsValid { get; set; }
        public string TouchValue { get; set; }
        public bool TouchIsValid { get; set; }
        public string HandPositionValue { get; set; }
        public bool HandPositionIsValid { get; set; }
        public string PositionValue { get; set; }
        public bool PositionIsValid { get; set; }

        // IsValid es true solo si todas las métricas son válidas
        public bool IsValid { get; set; }
    }
}