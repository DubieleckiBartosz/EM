using System;
using System.Collections.Generic;
using System.Linq;

namespace EventManagement.Application.Helpers
{
    public static class EnumHelpers
    {
        public static int EnumToInt<T>(this T value) where T : struct, IConvertible
        {
            CheckType<T>();
            return Convert.ToInt32(value);
        }

        public static List<string> GetStringValuesFromEnum<T>() where T : struct, IConvertible
        {
            CheckType<T>();
            var values =Enum.GetValues(typeof(T)).Cast<T>().Select(s=>s.ToString()).ToList();
            return values;
        }
        public static T ToEnum<T>(this string enumString)
        {
            CheckType<T>();
            return (T)Enum.Parse(typeof(T), enumString);
        }

        private static void CheckType<T>()
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type.");
            }
        }

        public static bool Exist<T>(this int value) => Enum.IsDefined(typeof(T), value);
    }
}