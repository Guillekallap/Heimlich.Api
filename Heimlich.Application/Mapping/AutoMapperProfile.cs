using AutoMapper;
using Heimlich.Application.DTOs;
using Heimlich.Domain.Entities;
using Heimlich.Domain.Enums;
using System.Linq;

namespace Heimlich.Application.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Group
            CreateMap<Group, GroupDto>()
                .ForMember(dest => dest.PractitionerIds, opt => opt.MapFrom(src => src.UserGroups.Select(ug => ug.UserId).ToList()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<CreateGroupDto, Group>()
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => GroupStatusEnum.Active));
            CreateMap<EditGroupDto, Group>();

            // Map Measurement -> EvaluationMeasurementDto (align with SimulationMeasurementDto shape)
            CreateMap<Measurement, EvaluationMeasurementDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => new DateTimeOffset(src.Time).ToUnixTimeMilliseconds()))
                .ForMember(dest => dest.ElapsedMs, opt => opt.MapFrom(src => src.ElapsedMs ?? 0))
                .ForMember(dest => dest.Result, opt => opt.MapFrom(src => src.IsValid ? "CORRECT" : "INCORRECT"))
                .ForMember(dest => dest.AngleDeg, opt => opt.MapFrom(src => src.AngleDeg ?? string.Empty))
                .ForMember(dest => dest.AngleStatus, opt => opt.MapFrom(src => src.AngleStatus))
                .ForMember(dest => dest.ForceValue, opt => opt.MapFrom(src => src.ForceValue ?? string.Empty))
                .ForMember(dest => dest.ForceStatus, opt => opt.MapFrom(src => src.ForceStatus))
                .ForMember(dest => dest.TouchStatus, opt => opt.MapFrom(src => src.TouchStatus))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src => src.IsValid));

            // Map Measurement -> SimulationMeasurementDto
            CreateMap<Measurement, SimulationMeasurementDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => new DateTimeOffset(src.Time).ToUnixTimeMilliseconds()))
                .ForMember(dest => dest.ElapsedMs, opt => opt.MapFrom(src => src.ElapsedMs ?? 0))
                .ForMember(dest => dest.Result, opt => opt.MapFrom(src => src.IsValid ? "CORRECT" : "INCORRECT"))
                .ForMember(dest => dest.AngleDeg, opt => opt.MapFrom(src => src.AngleDeg ?? string.Empty))
                .ForMember(dest => dest.AngleStatus, opt => opt.MapFrom(src => src.AngleStatus))
                .ForMember(dest => dest.ForceValue, opt => opt.MapFrom(src => src.ForceValue ?? string.Empty))
                .ForMember(dest => dest.ForceStatus, opt => opt.MapFrom(src => src.ForceStatus))
                .ForMember(dest => dest.TouchStatus, opt => opt.MapFrom(src => src.TouchStatus))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src => src.IsValid));

            // Evaluation mapping (incluir campos agregados)
            CreateMap<Evaluation, EvaluationDto>()
                .ForMember(dest => dest.EvaluatedUserFullName, opt => opt.MapFrom(src => src.EvaluatedUser != null ? src.EvaluatedUser.Fullname : null))
                .ForMember(dest => dest.Measurements, opt => opt.MapFrom(src => src.Measurements.OrderBy(m => m.ElapsedMs)))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate))
                .ForMember(dest => dest.ValidatedAt, opt => opt.MapFrom(src => src.ValidatedAt));

            // Keep Simulation mapping simple: map scalar fields; measurements are mapped by AutoMapper
            CreateMap<Simulation, SimulationSessionDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PractitionerId, opt => opt.MapFrom(src => src.PractitionerId))
                .ForMember(dest => dest.TrunkId, opt => opt.MapFrom(src => src.TrunkId))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate))
                .ForMember(dest => dest.TotalDurationMs, opt => opt.MapFrom(src => src.TotalDurationMs))
                .ForMember(dest => dest.TotalErrors, opt => opt.MapFrom(src => src.TotalErrors))
                .ForMember(dest => dest.TotalSuccess, opt => opt.MapFrom(src => src.TotalSuccess))
                .ForMember(dest => dest.TotalMeasurements, opt => opt.MapFrom(src => src.TotalMeasurements))
                .ForMember(dest => dest.SuccessRate, opt => opt.MapFrom(src => src.SuccessRate))
                .ForMember(dest => dest.AverageErrorsPerMeasurement, opt => opt.MapFrom(src => src.AverageErrorsPerMeasurement))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
                .ForMember(dest => dest.Measurements, opt => opt.MapFrom(src => src.Measurements.OrderBy(m => m.ElapsedMs)));
        }
    }
}