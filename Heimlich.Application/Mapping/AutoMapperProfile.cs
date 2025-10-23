using AutoMapper;
using Heimlich.Application.DTOs;
using Heimlich.Domain.Entities;
using Heimlich.Domain.Enums;

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

            // Measurement mapping for evaluations
            CreateMap<Measurement, EvaluationMeasurementDto>();

            // Evaluation mapping (incluir campos agregados)
            CreateMap<Evaluation, EvaluationDto>()
                .ForMember(dest => dest.Measurements, opt => opt.MapFrom(src => src.Measurements));

            // Simulation mapping centralizado
            CreateMap<Simulation, SimulationSessionDto>()
                .ForMember(dest => dest.AverageErrorsPerMeasurement, opt => opt.MapFrom(src => src.AverageErrorsPerMeasurement))
                .ForMember(dest => dest.Samples, opt => opt.MapFrom(src => src.Measurements
                    .OrderBy(m => m.ElapsedMs)
                    .Select(m => new SimulationSampleDto
                    {
                        ElapsedMs = m.ElapsedMs ?? 0,
                        Measurement = new SimulationMeasurementDto
                        {
                            ForceValue = m.ForceValue,
                            ForceIsValid = m.ForceIsValid,
                            TouchValue = m.TouchValue,
                            TouchIsValid = m.TouchIsValid,
                            HandPositionValue = m.HandPositionValue,
                            HandPositionIsValid = m.HandPositionIsValid,
                            PositionValue = m.PositionValue,
                            PositionIsValid = m.PositionIsValid,
                            IsValid = m.IsValid
                        }
                    }).ToList()));
        }
    }
}