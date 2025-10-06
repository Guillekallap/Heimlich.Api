using Heimlich.Application.DTOs;
using MediatR;

namespace Heimlich.Application.Features.Groups.Commands
{
    public class UpdateEvaluationParametersCommand : IRequest<GroupDto>
    {
        public int GroupId { get; }
        public List<SensorIntervalDto> SensorIntervals { get; }
        public int MaxErrors { get; }
        public int MaxTime { get; }

        public UpdateEvaluationParametersCommand(int groupId, List<SensorIntervalDto> sensorIntervals, int maxErrors, int maxTime)
        {
            GroupId = groupId;
            SensorIntervals = sensorIntervals;
            MaxErrors = maxErrors;
            MaxTime = maxTime;
        }
    }
}