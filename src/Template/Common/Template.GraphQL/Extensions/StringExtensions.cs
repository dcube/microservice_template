namespace Template.GraphQL.Extensions
{
    internal static class StringExtensions
    {
        public static string ToCamelCase(this string value)
        {
            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }
    }
}
