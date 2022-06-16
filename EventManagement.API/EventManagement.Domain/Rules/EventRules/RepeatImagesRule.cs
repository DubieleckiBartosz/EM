using System.Collections.Generic;
using System.Linq;
using EventManagement.Domain.Base;

namespace EventManagement.Domain.Rules.EventRules
{
    public class RepeatImagesRule : IBusinessRule
    {
        private readonly List<string> _images;
        private readonly string _newImage;

        public RepeatImagesRule(List<string> images, string newImage)
        {
            this._images = images;
            this._newImage = newImage;
        }

        public bool IsBroken()
        {
            var alreadyExists = this._images.Any(x => x == this._newImage);
            if (alreadyExists)
            {
                return true;
            }

            return false;
        }

        public string ErrorMessage => "Such an image already exists.";
    }
}