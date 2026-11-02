namespace QuickTools.Shared.Extensions
{
    public static class StringExtensions
    {
        public static string MaskString(
            this string value,
            int visibleStart = 2,
            int visibleEnd = 2,
            char maskChar = '*')
        {
            if (string.IsNullOrEmpty(value))
                return value;

            if (visibleStart < 0 || visibleEnd < 0)
                throw new ArgumentOutOfRangeException("Visible character counts must not be negative.");

            int length = value.Length;

            // Không đủ ký tự để masking
            if (length <= visibleStart + visibleEnd)
                return value;

            string start = value.Substring(0, visibleStart);
            string end = value.Substring(length - visibleEnd);
            int maskedLength = length - visibleStart - visibleEnd;

            return start + new string(maskChar, maskedLength) + end;
        }

    }
}