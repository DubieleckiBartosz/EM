namespace EventManagement.Domain.Base
{
    public interface IBusinessRule
    {
        bool IsBroken();
        string ErrorMessage { get; }
    }
}
