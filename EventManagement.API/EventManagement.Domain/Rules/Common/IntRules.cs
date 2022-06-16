using EventManagement.Domain.Base;

namespace EventManagement.Domain.Rules.Common
{
    public class IntRules : IBusinessRule
    {
        private readonly int _min;
        private readonly int _max;
        private readonly int _current;
        private readonly string _propName;

        public IntRules(int min, int max, int current, string prop)
        {
            this._min = min;
            this._max = max;
            this._current = current;
            this._propName = prop;
        }

        public string ErrorMessage => $"{_propName} should be between {this._min} and {this._max}";

        public bool IsBroken()
        {
            if (this._current < this._min || this._current > this._max)
            {
                return true;
            }

            return false;
        }
    }
}