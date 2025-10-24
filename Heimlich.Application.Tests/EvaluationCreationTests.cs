using AutoMapper;
using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Evaluations.Handlers;
using Heimlich.Domain.Entities;
using Heimlich.Application.Mapping;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;
using System;
using Heimlich.Infrastructure.Identity;

namespace Heimlich.Application.Tests
{
    public class EvaluationCreationTests
    {
        private readonly IMapper _mapper;

        public EvaluationCreationTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateEvaluation_WithClientAggregates_ShouldPersistAggregates()
        {
            var options = new DbContextOptionsBuilder<HeimlichDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_CreateEval_1_" + Guid.NewGuid())
                .Options;

            using var context = new HeimlichDbContext(options);
            var handler = new CreateEvaluationHandler(context, _mapper);

            var dto = new CreateEvaluationDto
            {
                EvaluatedUserId = "e1",
                TrunkId = 1,
                GroupId = 1,
                Score = 90,
                TotalDurationMs = 2000,
                TotalMeasurements = 2,
                TotalSuccess = 1,
                TotalErrors = 1,
                SuccessRate = 0.5,
                AverageErrorsPerMeasurement = 0.5,
                Measurements = new System.Collections.Generic.List<EvaluationMeasurementInputDto>
                {
                    new EvaluationMeasurementInputDto { Id = null, Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), ElapsedMs = 0, Result = "OK", AngleDeg = "7", AngleStatus = true, ForceValue = "10", ForceStatus = true, TouchStatus = true, Status = true, Message = "ok", IsValid = true },
                    new EvaluationMeasurementInputDto { Id = null, Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()+1000, ElapsedMs = 1000, Result = "NOK", AngleDeg = "8", AngleStatus = false, ForceValue = "8", ForceStatus = false, TouchStatus = false, Status = false, Message = "low", IsValid = false }
                }
            };

            var command = new Heimlich.Application.Features.Evaluations.Commands.CreateEvaluationCommand(dto, "instr1");
            var res = await handler.Handle(command, default);

            var saved = context.Evaluations.Include(e => e.Measurements).FirstOrDefault();
            Assert.NotNull(saved);
            Assert.Equal(2000, saved.TotalDurationMs);
            Assert.Equal(2, saved.TotalMeasurements);
            Assert.Equal(1, saved.TotalSuccess);
            Assert.Equal(1, saved.TotalErrors);
            Assert.Equal(0.5, saved.SuccessRate);
            Assert.Equal(0.5, saved.AverageErrorsPerMeasurement);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateEvaluation_WithoutClientAggregates_ShouldComputeAggregates()
        {
            var options = new DbContextOptionsBuilder<HeimlichDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_CreateEval_2_" + Guid.NewGuid())
                .Options;

            using var context = new HeimlichDbContext(options);
            var handler = new CreateEvaluationHandler(context, _mapper);

            var dto = new CreateEvaluationDto
            {
                EvaluatedUserId = "e2",
                TrunkId = 1,
                GroupId = 1,
                Score = 75,
                Measurements = new System.Collections.Generic.List<EvaluationMeasurementInputDto>
                {
                    new EvaluationMeasurementInputDto { Id = null, Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), ElapsedMs = 0, Result = "OK", AngleDeg = "7", AngleStatus = true, ForceValue = "10", ForceStatus = true, TouchStatus = true, Status = true, Message = "ok", IsValid = true },
                    new EvaluationMeasurementInputDto { Id = null, Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()+1000, ElapsedMs = 1000, Result = "NOK", AngleDeg = "8", AngleStatus = false, ForceValue = "8", ForceStatus = false, TouchStatus = false, Status = false, Message = "low", IsValid = false }
                }
            };

            var command = new Heimlich.Application.Features.Evaluations.Commands.CreateEvaluationCommand(dto, "instr1");
            var res = await handler.Handle(command, default);

            var saved = context.Evaluations.Include(e => e.Measurements).FirstOrDefault();
            Assert.NotNull(saved);
            Assert.Equal(2, saved.TotalMeasurements);
            Assert.Equal(1, saved.TotalSuccess);
            Assert.Equal(1, saved.TotalErrors);
            Assert.Equal(0.5, saved.SuccessRate);
            Assert.Equal(0.5, saved.AverageErrorsPerMeasurement);
            Assert.True(saved.TotalDurationMs >= 1000);
        }
    }
}
