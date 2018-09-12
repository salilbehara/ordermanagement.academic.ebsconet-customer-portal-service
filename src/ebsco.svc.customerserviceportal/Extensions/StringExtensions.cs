namespace ebsco.svc.customerserviceportal.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Remove all the leading and trailing white-space characters from the string. Handle Null values.
        /// </summary>
        public static string NullSafeTrim(this string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : value.Trim();
        }
    }
}
