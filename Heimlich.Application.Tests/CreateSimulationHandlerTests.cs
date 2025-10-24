using AutoMapper;
using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Simulations.Commands;
using Heimlich.Application.Features.Simulations.Handlers;
using Heimlich.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

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
                Measurements = new List<SimulationMeasurementDto>
                {
                    new SimulationMeasurementDto { ElapsedMs = 0, ForceValue = "10", ForceStatus = true, TouchStatus = true, AngleDeg = "7", AngleStatus = true, Status = true, IsValid = true },
                    new SimulationMeasurementDto { ElapsedMs = 0, ForceValue = "11", ForceStatus = false, TouchStatus = false, AngleDeg = "8", AngleStatus = false, Status = false, IsValid = false },
                    new SimulationMeasurementDto { ElapsedMs = 1000, ForceValue = "10", ForceStatus = true, TouchStatus = true, AngleDeg = "7", AngleStatus = true, Status = true, IsValid = true }
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