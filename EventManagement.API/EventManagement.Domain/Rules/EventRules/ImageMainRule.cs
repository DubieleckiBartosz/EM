using System.Collections.Generic;
using System.Linq;
using EventManagement.Domain.Base;
using EventManagement.Domain.Entities;

namespace EventManagement.Domain.Rules.EventRules
{
    public class ImageMainRule : IBusinessRule
    {
        private readonly bool _isMainStatusCreating;
        private readonly bool _imagesNotNullAndNotEmpty;
        public ImageMainRule(List<EventImage> images, bool isMainStatusCreating)
        {
            this._isMainStatusCreating = isMainStatusCreating;
            this._imagesNotNullAndNotEmpty = images != null && images.Any();
        }
        public bool IsBroken()
        {

            if (!this._imagesNotNullAndNotEmpty && !this._isMainStatusCreating)
            {
                return true;
            }

            return false;
        }

        public string ErrorMessage => "The image must be the main image when the image list is null or empty.";
    }
}
