using System;
using System.Linq;

namespace ImageGallery.Client.Helpers
{
    /// <summary>
    ///
    /// </summary>
    public static class HttpHelpers
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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
