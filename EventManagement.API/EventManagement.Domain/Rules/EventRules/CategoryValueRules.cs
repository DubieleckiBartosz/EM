using EventManagement.Domain.Base;
using EventManagement.Domain.Rules.Common;

namespace EventManagement.Domain.Rules.EventRules
{
    public class CategoryValueRules : BaseRules, IBusinessRule
    {
        private readonly string _categoryName;
        private readonly string _categoryType;

        public CategoryValueRules(string categoryName, string categoryType)
        {
            this._categoryName = categoryName;
            this._categoryType = categoryType;
        }

        public bool IsBroken()
        {
            if (string.IsNullOrEmpty(this._categoryName))
            {
                this.Error = this.GetErrorWhenEmptyOrNull("Category name");
                return true;
            }

            if (string.IsNullOrEmpty(this._categoryType))
            {
                this.Error = this.GetErrorWhenEmptyOrNull("Category type");
                return true;
            }

            return false;
        }

        public new string ErrorMessage => this.Error;
    }
}