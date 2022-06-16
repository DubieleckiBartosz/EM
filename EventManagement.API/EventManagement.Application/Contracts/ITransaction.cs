using System.Data;
using System.Threading.Tasks;

namespace EventManagement.Application.Contracts
{
    public interface ITransaction
    {
        IDbTransaction GetTransactionWhenExist();
        Task<IDbTransaction> GetOpenOrCreateTransaction();
        bool Commit();
        void Rollback();
    }
} 