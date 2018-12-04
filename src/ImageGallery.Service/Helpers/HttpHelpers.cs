using System;
using System.Linq;

namespace ImageGallery.Service.Helpers
{
    public static class HttpHelpers
    {
        public static string ToQueryString(this object obj)
        {
            var result = string.Join("&", obj.GetType()
                .GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(QueryStringAttribute)) && p.GetValue(obj, null) != null)
                .Select(p => $"{Uri.EscapeDataString(p.Name)}={Uri.EscapeDataString(p.GetValue(obj).ToString())}"));

            if (result.Length != 0)
            {
                return $"?{result}";
            }

            return string.Empty;
        }
    }
}
