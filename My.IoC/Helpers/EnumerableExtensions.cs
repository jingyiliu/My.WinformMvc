using System.Collections.Generic;

namespace My.Helpers
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class EnumerableExtensions
    {
        public static IList<T> ToList<T>(this IEnumerable<T> parameters)
        {
            Requires.NotNull(parameters, "parameters");
            return new List<T>(parameters);
        }
    }
}
