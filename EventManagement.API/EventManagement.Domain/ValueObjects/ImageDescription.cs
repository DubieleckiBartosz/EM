using System.Collections.Generic;
using EventManagement.Domain.DDD;
using EventManagement.Domain.Rules;
using EventManagement.Domain.Rules.Common;

namespace EventManagement.Domain.ValueObjects
{
    public class ImageDescription : ValueObject
    {
        public string Description { get; private set; }

        private ImageDescription(string imageDescription)
        {
            this.Description = imageDescription;
        }

        public static ImageDescription Create(string imageDescription)
        {
            CheckRule(new StringRules(imageDescription, nameof(Description), imageDescription?.Length >= 10,
                customError: "Description length should be longer than 10 characters."));
            return new ImageDescription(imageDescription);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Description;
        }
    }
}