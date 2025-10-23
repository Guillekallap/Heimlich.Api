using Xunit;
using Moq;
using Heimlich.Api.Controllers;
using Heimlich.Application.DTOs;
using Heimlich.Application.Features.Groups.Commands;
using Heimlich.Application.Features.Evaluations.Commands;
using Heimlich.Domain.Entities;
using Heimlich.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Heimlich.Api.Tests.Controllers
{
    public class InstructorControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly HeimlichDbContext _contextMock;
        private readonly InstructorController _controller;

        public InstructorControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _contextMock = null; // No se usa en los tests actuales
            _controller = new InstructorController(_mediatorMock.Object, _contextMock);
        }

        private void SetUser(string userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task CreateGroup_ReturnsOk()
        {
            var dto = new CreateGroupDto
            {
                Name = "Grupo Test",
                Description = "Desc",
                PractitionerIds = new List<string> { "user1", "user2" },
                EvaluationConfigId = 1
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateGroupCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GroupDto { Id = 1, Name = dto.Name });

            var result = await _controller.CreateGroup(dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var group = Assert.IsType<GroupDto>(okResult.Value);
            Assert.Equal("Grupo Test", group.Name);
        }

        [Fact]
        public async Task EditGroup_ReturnsOk()
        {
            var dto = new EditGroupDto
            {
                Name = "Grupo Editado",
                Description = "Desc Editada",
                PractitionerIds = new List<string> { "user1" }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<EditGroupCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GroupDto { Id = 1, Name = dto.Name });

            var result = await _controller.EditGroup(1, dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var group = Assert.IsType<GroupDto>(okResult.Value);
            Assert.Equal("Grupo Editado", group.Name);
        }

        [Fact]
        public async Task DeleteGroup_ReturnsNoContent()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteGroupCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);
            var result = await _controller.DeleteGroup(1);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CreateEvaluation_ReturnsOk()
        {
            var dto = new CreateEvaluationDto
            {
                EvaluatedUserId = "user1",
                TrunkId = 1,
                GroupId = 1,
                Comments = "Test eval",
                Score = 90,
                Measurements = new List<EvaluationMeasurementInputDto>
                {
                    new EvaluationMeasurementInputDto { ElapsedMs = 0, ForceValue = "10", ForceIsValid = true, TouchValue = "5", TouchIsValid = true, HandPositionValue = "7", HandPositionIsValid = true, PositionValue = "8", PositionIsValid = true, IsValid = true },
                    new EvaluationMeasurementInputDto { ElapsedMs = 1000, ForceValue = "11", ForceIsValid = false, TouchValue = "6", TouchIsValid = true, HandPositionValue = "8", HandPositionIsValid = true, PositionValue = "9", PositionIsValid = true, IsValid = false },
                    new EvaluationMeasurementInputDto { ElapsedMs = 2000, ForceValue = "12", ForceIsValid = true, TouchValue = "7", TouchIsValid = true, HandPositionValue = "9", HandPositionIsValid = true, PositionValue = "10", PositionIsValid = true, IsValid = true },
                    new EvaluationMeasurementInputDto { ElapsedMs = 3000, ForceValue = "13", ForceIsValid = true, TouchValue = "8", TouchIsValid = true, HandPositionValue = "10", HandPositionIsValid = true, PositionValue = "11", PositionIsValid = true, IsValid = true }
                }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateEvaluationCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EvaluationDto { Id = 1, Score = 90 });

            SetUser("instructor1");
            var result = await _controller.CreateEvaluation(dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var eval = Assert.IsType<EvaluationDto>(okResult.Value);
            Assert.Equal(90, eval.Score);
        }

        [Fact]
        public async Task EditGroup_ReturnsOk_WithDifferentData()
        {
            var dto = new EditGroupDto
            {
                Name = "Grupo B",
                Description = "Desc B",
                PractitionerIds = new List<string> { "user3", "user4" }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<EditGroupCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GroupDto { Id = 2, Name = dto.Name });

            var result = await _controller.EditGroup(2, dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var group = Assert.IsType<GroupDto>(okResult.Value);
            Assert.Equal("Grupo B", group.Name);
        }

        [Fact]
        public async Task CreateGroup_ReturnsOk_WithDifferentPractitioners()
        {
            var dto = new CreateGroupDto
            {
                Name = "Grupo C",
                Description = "Desc C",
                PractitionerIds = new List<string> { "user5" },
                EvaluationConfigId = 2
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateGroupCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GroupDto { Id = 3, Name = dto.Name });

            var result = await _controller.CreateGroup(dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var group = Assert.IsType<GroupDto>(okResult.Value);
            Assert.Equal("Grupo C", group.Name);
        }

        [Fact]
        public async Task CreateEvaluation_ReturnsOk_WithFailedMeasurements()
        {
            var dto = new CreateEvaluationDto
            {
                EvaluatedUserId = "user2",
                TrunkId = 2,
                GroupId = 2,
                Comments = "Evaluación con fallos",
                Score = 70,
                Measurements = new List<EvaluationMeasurementInputDto>
                {
                    new EvaluationMeasurementInputDto { ElapsedMs = 0, ForceValue = "10", ForceIsValid = false, TouchValue = "5", TouchIsValid = false, HandPositionValue = "7", HandPositionIsValid = false, PositionValue = "8", PositionIsValid = false, IsValid = false },
                    new EvaluationMeasurementInputDto { ElapsedMs = 1000, ForceValue = "11", ForceIsValid = false, TouchValue = "6", TouchIsValid = false, HandPositionValue = "8", HandPositionIsValid = false, PositionValue = "9", PositionIsValid = false, IsValid = false },
                    new EvaluationMeasurementInputDto { ElapsedMs = 2000, ForceValue = "12", ForceIsValid = false, TouchValue = "7", TouchIsValid = false, HandPositionValue = "9", HandPositionIsValid = false, PositionValue = "10", PositionIsValid = false, IsValid = false },
                    new EvaluationMeasurementInputDto { ElapsedMs = 3000, ForceValue = "13", ForceIsValid = false, TouchValue = "8", TouchIsValid = false, HandPositionValue = "10", HandPositionIsValid = false, PositionValue = "11", PositionIsValid = false, IsValid = false }
                }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateEvaluationCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EvaluationDto { Id = 2, Score = 70 });

            SetUser("instructor2");
            var result = await _controller.CreateEvaluation(dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var eval = Assert.IsType<EvaluationDto>(okResult.Value);
            Assert.Equal(70, eval.Score);
        }
    }
}
