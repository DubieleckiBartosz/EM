using EventManagement.Domain.Base;
using EventManagement.Domain.Rules.Common;

namespace EventManagement.Domain.Rules.EventRules
{
    public class CreateAddressRules : BaseRules, IBusinessRule
    {
        private readonly string _city;
        private readonly string _street;
        private readonly string _numberStreet;
        private readonly string _postalCode;

        public CreateAddressRules(string city, string street, string numberStreet, string postalCode)
        {
            this._city = city;
            this._street = street;
            this._postalCode = postalCode;
            this._numberStreet = numberStreet;
        }

        public bool IsBroken()
        {
            if (string.IsNullOrEmpty(this._city))
            {
                this.GetErrorWhenEmptyOrNull("City");
                return true;
            }

            if (string.IsNullOrEmpty(this._street))
            {
                this.GetErrorWhenEmptyOrNull("Street");

                return true;
            }

            if (string.IsNullOrEmpty(this._postalCode))
            {
                this.GetErrorWhenEmptyOrNull("Postal code");

                return true;
            }

            if (string.IsNullOrEmpty(this._numberStreet))
            {
                this.GetErrorWhenEmptyOrNull("Number street");

                return true;
            }

            var cityLength = this._city.Length;
            var streetLength = this._street.Length;
            var postalCodeLength = this._postalCode.Length;
            var numberStreetLength = this._numberStreet.Length;

            if (cityLength < 3 || cityLength > 50)
            {
                this.Error = $"City length should be between 3 and 50";
                return true;
            }

            if (streetLength < 3 || streetLength > 50)
            {
                this.Error = $"Street length should be between 3 and 50";
                return true;
            }

            if (postalCodeLength < 3 || postalCodeLength > 10)
            {
                this.Error = $"Postal code length should be between 3 and 10";
                return true;
            }

            if (numberStreetLength < 3 || numberStreetLength > 10)
            {
                this.Error = $"Number street length should be between 3 and 10";
                return true;
            }

            return false;
        }

        public new string ErrorMessage => this.Error;
    }
}