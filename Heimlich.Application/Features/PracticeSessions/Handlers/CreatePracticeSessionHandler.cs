using AutoMapper;
using Heimlich.Application.DTOs;
using Heimlich.Application.Features.PracticeSessions.Commands;
using Heimlich.Domain.Entities;
using Heimlich.Domain.Enums;
using Heimlich.Infrastructure.Identity;
using MediatR;

namespace Heimlich.Application.Features.PracticeSessions.Handlers
{
    public class CreatePracticeSessionHandler : IRequestHandler<CreatePracticeSessionCommand, PracticeSessionDto>
    {
        private readonly HeimlichDbContext _context;
        private readonly IMapper _mapper;

        public CreatePracticeSessionHandler(HeimlichDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PracticeSessionDto> Handle(CreatePracticeSessionCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.PractitionerId))
                throw new ArgumentException("El PractitionerId es obligatorio.");

            if (request.PracticeType == PracticeTypeEnum.Simulation && request.Dto.TrunkId <= 0)
                throw new ArgumentException("Debe especificar un TrunkId válido para la simulación.");

            // Mapear DTO a entidad
            var session = _mapper.Map<PracticeSession>(request.Dto);
            session.PractitionerId = request.PractitionerId;
            session.PracticeType = request.PracticeType;
            session.Measurements = new List<Measurement>();
            session.Evaluations = new List<Evaluation>();

            session.GroupId = request.PracticeType == PracticeTypeEnum.Simulation ? null : request.Dto.GroupId;

            _context.PracticeSessions.Add(session);
            await _context.SaveChangesAsync(cancellationToken);

            // Mapear entidad a DTO
            var dto = _mapper.Map<PracticeSessionDto>(session);
            return dto;
        }
    }
}