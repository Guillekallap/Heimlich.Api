using MediatR;
using Heimlich.Domain.Entities;

namespace Heimlich.Application.Features.Evaluations.Commands
{
    public class UpsertEvaluationConfigCommand : IRequest<EvaluationConfig>
    {
        public int GroupId { get; }
        public int MaxErrors { get; }
        public int MaxTime { get; }
        public string Name { get; }
        public bool IsDefault { get; }
        public UpsertEvaluationConfigCommand(int groupId, int maxErrors, int maxTime, string name, bool isDefault)
        {
            GroupId = groupId;
            MaxErrors = maxErrors;
            MaxTime = maxTime;
            Name = name;
            IsDefault = isDefault;
        }
    }
}
