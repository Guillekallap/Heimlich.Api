using AutoMapper;
using Heimlich.Domain.Entities;
using Heimlich.Application.DTOs;

namespace Heimlich.Application.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // PracticeSession
            CreateMap<PracticeSession, PracticeSessionDto>().ReverseMap();
            CreateMap<PracticeSession, CancelPracticeSessionDto>();
            CreateMap<CreatePracticeSessionDto, PracticeSession>()
                .ForMember(dest => dest.Measurements, opt => opt.Ignore())
                .ForMember(dest => dest.Evaluations, opt => opt.Ignore())
                .ForMember(dest => dest.PractitionerId, opt => opt.Ignore())
                .ForMember(dest => dest.PracticeType, opt => opt.Ignore());

            // Group
            CreateMap<Group, GroupDto>()
                .ForMember(dest => dest.PractitionerIds, opt => opt.MapFrom(src => src.UserGroups.Select(ug => ug.UserId).ToList()));
            CreateMap<CreateGroupDto, Group>()
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Activo"));
            CreateMap<EditGroupDto, Group>();

            // Evaluation
            CreateMap<Evaluation, EvaluationDto>();
        }
    }
}