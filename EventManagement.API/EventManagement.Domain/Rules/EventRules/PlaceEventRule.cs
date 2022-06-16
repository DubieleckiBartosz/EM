using EventManagement.Domain.Base;
using EventManagement.Domain.Rules.Common;

namespace EventManagement.Domain.Rules.EventRules
{
    public class PlaceEventRule : BaseRules, IBusinessRule
    {
        private readonly string _city;
        private readonly string _postalCode;
        private readonly string _street;
        private readonly string _numberStreet;


        public PlaceEventRule(string city, string postalCode, string street, string numberStreet)
        {
            this._city = city;
            this._postalCode = postalCode;
            this._street = street;
            this._numberStreet = numberStreet;
        }

        public bool IsBroken()
        {
            if (string.IsNullOrEmpty(this._city))
            {
                this.Error = this.GetErrorWhenEmptyOrNull("City");
                return true;
            }

            if (string.IsNullOrEmpty(this._postalCode))
            {
                this.Error = this.GetErrorWhenEmptyOrNull("Postal code");
                return true;
            }

            if (string.IsNullOrEmpty(this._street))
            {
                this.Error = this.GetErrorWhenEmptyOrNull("Street");
                return true;
            }

            if (string.IsNullOrEmpty(this._numberStreet))
            {
                this.Error = this.GetErrorWhenEmptyOrNull("Number street");
                return true;
            }

            return false;
        }

      
    }
}