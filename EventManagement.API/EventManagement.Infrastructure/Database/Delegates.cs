namespace EventManagement.Infrastructure.Database
{
    public class Delegates
    {
        public delegate bool CommitTransaction();
        public delegate void RollbackTransaction();
    }
}
