using System.Threading;
using System.Threading.Tasks;

namespace EventManagement.Domain.Base
{
    public interface IDomainDecorator
    {
        Task Publish<TNotification>(TNotification notification,
            CancellationToken cancellationToken = default(CancellationToken))
            where TNotification : IDomainNotification;
    }
}