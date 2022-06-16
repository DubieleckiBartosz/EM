namespace EventManagement.Domain.Rules.Common
{
    public abstract class BaseRules
    {
        protected string Error = string.Empty;
        protected string GetErrorWhenEmptyOrNull(string prop) => string.Format("{0} should not be null and should not be empty.", prop);
        public string ErrorMessage => this.Error;
    }
}
