using System;
using EventManagement.Domain.Base;

namespace EventManagement.Domain.Rules.Common
{
    public class StringRules : BaseRules, IBusinessRule
    {
        private readonly string _propValue;
        private readonly string _propNameForError;
        private readonly string _customError;
        private readonly bool _canBeNull;
        private readonly Func<string, bool> _func;
        private readonly bool _customRuleOk;

        public StringRules(string propValue, string propNameForError, bool customRuleOk = true,
            Func<string, bool> func = null,
            string customError = null, bool canBeNull = false)
        {
            this._propValue = propValue;
            this._propNameForError = propNameForError;
            this._func = func;
            this._customError = customError;
            this._canBeNull = canBeNull;
            this._customRuleOk = customRuleOk;
        }

        public bool IsBroken()
        {
            if (this._canBeNull && string.IsNullOrEmpty(this._propValue))
            {
                return false;
            }

            if (!this._canBeNull && string.IsNullOrEmpty(this._propValue))
            {
                this.Error = this.GetErrorWhenEmptyOrNull(this._propNameForError);
                return true;
            }

            if (!this._customRuleOk)
            {
                this.Error = this._customError;
                return true;
            }

            if (this._func == null)
            {
                return false;
            }

            var success = this._func(_propValue);
            if (success)
            {
                return false;
            }
            this.Error = this._customError ?? "The condition for the delegate was not met.";
            return true;

        }

        public new string ErrorMessage => this.Error;
    }
}