using System;
using EventManagement.Application.Strings.Responses;

namespace EventManagement.Application.Helpers
{
    public static class ConvertTypeHelpers
    {
        public static int? ToNullableInt(this string value)
        {
            if (int.TryParse(value, out var valueIntResult))
            {
                return valueIntResult;
            }

            return null;
        }

        public static DateTime ToDateTime(this string dateString)
        {
            if (DateTime.TryParse(dateString, out DateTime dateValue))
            {
                return dateValue;
            }

            throw new ArgumentException(ResponseStrings.InvalidType(dateString));
        }
        public static int ToInt(this string value)
        {
            if (int.TryParse(value, out var valueIntResult))
            {
                return valueIntResult;
            }

            throw new ArgumentException(ResponseStrings.InvalidType(value));
        }
    }
}