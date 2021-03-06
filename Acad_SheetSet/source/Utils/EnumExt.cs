﻿namespace Acad_SheetSet.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumExt
    {
        /// <summary>
        /// Атрибут Description из значения enum
        /// </summary>
        /// <param name="enumValue">Enum значение</param>
        /// <returns>Подаись в атрибуте Descrip</returns>
        public static string Description(this object enumValue)
        {
            return enumValue == null ? null : EnumDescriptionTypeConverter.GetEnumDescription(enumValue);
        }

        public static List<string> GetEnumDesciptionValues(this Enum value)
        {
            return Enum.GetValues(value.GetType()).Cast<Enum>().Select(s => s.Description()).ToList();
        }

        /// <summary>
        /// Есть ли хоть один совпадающий флаг в enum1 и enum2
        /// </summary>
        public static bool HasAny<T>(this T enum1, T enum2) where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("Это не enum");

            var v1 = Convert.ToInt32(enum1);
            var v2 = Convert.ToInt32(enum2);
            return v1 == 0 || v2 == 0 || (v1 & v2) > 0;
        }

        /// <summary>
        /// Конвертация строки в соответствующее значение перечисления enum
        /// Выбрасывает исключение при несоответствии
        /// </summary>
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Конвертация строки в соответствующее значение перечисления enum.
        /// Ичключение не выбрасывапется. (если, только, T не структура)
        /// </summary>
        public static T ToEnum<T>(this string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return Enum.TryParse(value, true, out T result) ? result : defaultValue;
        }
    }
}