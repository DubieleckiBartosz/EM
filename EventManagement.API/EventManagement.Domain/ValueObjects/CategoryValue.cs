using System.Collections.Generic;
using EventManagement.Domain.DDD;
using EventManagement.Domain.Rules;
using EventManagement.Domain.Rules.EventRules;

namespace EventManagement.Domain.ValueObjects
{
    public class CategoryValue : ValueObject
    {
        public string CategoryName { get; private set; }
        public string CategoryType { get; private set; }

        private CategoryValue(string categoryName, string categoryType)
        {
            (this.CategoryName, this.CategoryType) = (categoryName, categoryType);
        }

        public static CategoryValue Create(string categoryName, string categoryType)
        {
            CheckRule(new CategoryValueRules(categoryName, categoryType));
            return new CategoryValue(categoryName, categoryType);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.CategoryName;
            yield return this.CategoryType;
        }
    }
}