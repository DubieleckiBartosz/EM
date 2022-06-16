using System.Threading.Tasks;
using EventManagement.Application.Contracts.Repositories;
using EventManagement.Domain.DDD;

namespace EventManagement.Application.Contracts
{
    public interface IUnitOfWork
    {
        IEventApplicationRepository EventApplication { get; }
        IEventRepository EventRepository { get; }
        IPerformerRepository Performer { get; }
        IEventImageRepository EventImageRepository { get; }
        IOpinionRepository OpinionRepository { get; }
        IPerformanceProposalRepository ProposalRepository { get; }
        void Complete();
        Task CompleteAsync<T>(T aggregate) where T : AggregateRoot;
    }
}
