namespace CmdTools.Shared
{
    using System.ComponentModel;

    /// <summary>
    /// Extension class for enums
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Get a description attribute for a enum
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <returns>Description attribute</returns>
        public static string GetDescription<T>(this T enumValue)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException($"{typeof(T).Name} must be a enum");
            }

            var description = enumValue.ToString();
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    description = ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return description;
        }
    }
}
