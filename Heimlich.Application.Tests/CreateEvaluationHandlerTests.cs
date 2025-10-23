using AutoMapper;
using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Commands;
using Heimlich.Application.Features.Evaluations.Handlers;
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
    public class CreateEvaluationHandlerTests
    {
        private readonly IMapper _mapper;

        public CreateEvaluationHandlerTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new Heimlich.Application.Mapping.AutoMapperProfile()));
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task NormalizeMeasurements_KeepsSingleMeasurementPerElapsedMs()
        {
            var options = new DbContextOptionsBuilder<HeimlichDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_Eval_" + Guid.NewGuid())
                .Options;

            using var context = new HeimlichDbContext(options);
            var handler = new CreateEvaluationHandler(context, _mapper);

            var dto = new CreateEvaluationDto
            {
                EvaluatedUserId = "e1",
                TrunkId = 1,
                GroupId = 1,
                Score = 80,
                Measurements = new List<EvaluationMeasurementInputDto>
                {
                    new EvaluationMeasurementInputDto { ElapsedMs = 0, ForceValue = "10", ForceIsValid = true, TouchValue = "5", TouchIsValid = true, HandPositionValue = "7", HandPositionIsValid = true, PositionValue = "8", PositionIsValid = true, IsValid = true },
                    new EvaluationMeasurementInputDto { ElapsedMs = 0, ForceValue = "11", ForceIsValid = false, TouchValue = "6", TouchIsValid = false, HandPositionValue = "8", HandPositionIsValid = false, PositionValue = "9", PositionIsValid = false, IsValid = false },
                    new EvaluationMeasurementInputDto { ElapsedMs = 1000, ForceValue = "10", ForceIsValid = true, TouchValue = "5", TouchIsValid = true, HandPositionValue = "7", HandPositionIsValid = true, PositionValue = "8", PositionIsValid = true, IsValid = true }
                }
            };

            var command = new CreateEvaluationCommand(dto, "instr1");

            var result = await handler.Handle(command, CancellationToken.None);

            var savedEval = context.Evaluations.Include(e => e.Measurements).FirstOrDefault();
            Assert.NotNull(savedEval);
            Assert.Equal(2, savedEval.Measurements.Count);
            Assert.Equal(2, savedEval.TotalMeasurements);
        }
    }
}
