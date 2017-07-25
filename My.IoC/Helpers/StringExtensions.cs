namespace My.Helpers
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return true;
            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i]))
                    return false;
            }
            return true;
        }
    }
}
