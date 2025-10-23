using AutoMapper;
using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Simulations.Commands;
using Heimlich.Application.Features.Simulations.Handlers;
using Heimlich.Domain.Entities;
using Heimlich.Infrastructure.Identity;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Heimlich.Application.Tests
{
    public class CreateSimulationHandlerTests
    {
        private readonly IMapper _mapper;

        public CreateSimulationHandlerTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new Heimlich.Application.Mapping.AutoMapperProfile()));
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task NormalizeSamples_KeepsSingleSamplePerElapsedMs()
        {
            var options = new DbContextOptionsBuilder<HeimlichDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_Sim_" + Guid.NewGuid())
                .Options;

            using var context = new HeimlichDbContext(options);
            var handler = new CreateSimulationHandler(context, _mapper);

            var dto = new CreateSimulationDto
            {
                TrunkId = 1,
                Samples = new List<SimulationSampleDto>
                {
                    new SimulationSampleDto { ElapsedMs = 0, Measurement = new SimulationMeasurementDto { ForceValue = "10", ForceIsValid = true, TouchValue = "5", TouchIsValid = true, HandPositionValue = "7", HandPositionIsValid = true, PositionValue = "8", PositionIsValid = true, IsValid = true } },
                    new SimulationSampleDto { ElapsedMs = 0, Measurement = new SimulationMeasurementDto { ForceValue = "11", ForceIsValid = false, TouchValue = "6", TouchIsValid = false, HandPositionValue = "8", HandPositionIsValid = false, PositionValue = "9", PositionIsValid = false, IsValid = false } },
                    new SimulationSampleDto { ElapsedMs = 1000, Measurement = new SimulationMeasurementDto { ForceValue = "10", ForceIsValid = true, TouchValue = "5", TouchIsValid = true, HandPositionValue = "7", HandPositionIsValid = true, PositionValue = "8", PositionIsValid = true, IsValid = true } }
                }
            };

            var command = new CreateSimulationCommand(dto, "prac1");

            var result = await handler.Handle(command, CancellationToken.None);

            var savedSim = context.Simulations.Include(s => s.Measurements).FirstOrDefault();
            Assert.NotNull(savedSim);
            Assert.Equal(2, savedSim.Measurements.Count);
            Assert.Equal(2, savedSim.TotalMeasurements);
        }
    }
}
