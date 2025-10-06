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

            // Evaluation mapping (incluir campos agregados)
            CreateMap<Evaluation, EvaluationDto>();

            // Simulation mapping centralizado
            CreateMap<Simulation, SimulationSessionDto>()
                .ForMember(dest => dest.AverageErrorsPerSample, opt => opt.MapFrom(src => src.AverageErrorsPerMeasurement))
                .ForMember(dest => dest.Samples, opt => opt.MapFrom(src => src.Measurements
                    .GroupBy(m => m.ElapsedMs ?? 0)
                    .Select(g => new SimulationSampleDto
                    {
                        ElapsedMs = g.Key,
                        Metrics = g.Select(m => new SimulationMetricDto
                        {
                            MetricType = m.MetricType,
                            Value = m.Value,
                            IsValid = m.IsValid
                        }).ToList()
                    }).ToList()));
        }
    }
}