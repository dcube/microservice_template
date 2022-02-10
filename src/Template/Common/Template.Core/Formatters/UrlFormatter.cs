using Refit;
using System.Reflection;

namespace Template.Core.Formatters
{
    public class UrlFormatter : DefaultUrlParameterFormatter
    {
        public override string Format(object value, ICustomAttributeProvider attributeProvider, Type type)
        {
            if (value != null && typeof(DateTime?).IsAssignableFrom(value.GetType()))
            {
                return ((DateTime)value).ToString("yyyy-MM-dd");
            }

            return base.Format(value, attributeProvider, type);

        }
    }
}
