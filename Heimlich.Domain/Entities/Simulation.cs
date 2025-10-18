using System;
using System.Collections.Generic;
using Heimlich.Domain.Enums;

namespace Heimlich.Domain.Entities
{
    public class Simulation
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public string PractitionerId { get; set; }
        public User Practitioner { get; set; }
        public int TrunkId { get; set; }
        public Trunk Trunk { get; set; }
        public long TotalDurationMs { get; set; }
        public int TotalErrors { get; set; }
        public int TotalSuccess { get; set; }
        public int TotalMeasurements { get; set; }
        public double SuccessRate { get; set; }
        public double AverageErrorsPerMeasurement { get; set; }
        public bool? IsValid { get; set; }
        public string? Comments { get; set; }
        public SessionStateEnum State { get; set; } = SessionStateEnum.Active;
        public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
    }
}