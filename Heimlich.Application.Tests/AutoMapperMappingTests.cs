using AutoMapper;
using Heimlich.Application.Mapping;
using Heimlich.Application.DTOs;
using Heimlich.Domain.Entities;
using System;
using Xunit;
using System.Linq;

namespace Heimlich.Application.Tests
{
    public class AutoMapperMappingTests
    {
        private readonly IMapper _mapper;

        public AutoMapperMappingTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void Measurement_maps_to_SimulationMeasurementDto_and_EvaluationMeasurementDto()
        {
            var time = DateTime.UtcNow;
            var measurement = new Measurement
            {
                Id = 42,
                Time = time,
                ElapsedMs = 1500,
                ForceValue = "12.3",
                ForceStatus = true,
                TouchStatus = false,
                AngleDeg = "45",
                AngleStatus = true,
                Message = "pos-msg",
                Status = true,
                IsValid = true
            };

            var sim = new Simulation
            {
                Id = 7,
                PractitionerId = "prac1",
                TrunkId = 2,
                TotalDurationMs = 2000,
                TotalErrors = 0,
                TotalSuccess = 1,
                TotalMeasurements = 1,
                SuccessRate = 1.0,
                AverageErrorsPerMeasurement = 0,
                Comments = "c",
            };
            sim.Measurements.Add(measurement);

            var simDto = _mapper.Map<SimulationSessionDto>(sim);
            Assert.NotNull(simDto);
            Assert.Single(simDto.Measurements);
            var mapped = simDto.Measurements.First();
            Assert.Equal(measurement.Id, mapped.Id);
            Assert.Equal(measurement.ElapsedMs ?? 0, mapped.ElapsedMs);
            Assert.Equal("CORRECT", mapped.Result);
            Assert.Equal(measurement.AngleDeg, mapped.AngleDeg);
            Assert.Equal(measurement.ForceValue, mapped.ForceValue);
            Assert.Equal(measurement.Message, mapped.Message);
            Assert.Equal(measurement.IsValid, mapped.IsValid);

            var evalMapped = _mapper.Map<EvaluationMeasurementDto>(measurement);
            Assert.Equal(measurement.ElapsedMs, evalMapped.ElapsedMs);
            Assert.Equal(measurement.ForceValue, evalMapped.ForceValue);
            // Updated assertions for unified names
            Assert.Equal(measurement.ForceStatus, evalMapped.ForceStatus);
            Assert.Equal(measurement.AngleDeg, evalMapped.AngleDeg);
            Assert.Equal(measurement.Message, evalMapped.Message);
            Assert.Equal(measurement.IsValid, evalMapped.IsValid);
        }
    }
}
