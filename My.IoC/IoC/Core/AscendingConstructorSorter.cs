
using System.Reflection;
using System.Collections.Generic;
using My.Helpers;

namespace My.IoC.Core
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AscendingConstructorSorter : IConstructorSorter
    {
        private class ConstructorInfoComparer : IComparer<ConstructorInfo>
        {
            public int Compare(ConstructorInfo x, ConstructorInfo y)
            {
                var xParams = x.GetParameters();
                var yParams = y.GetParameters();
                if (xParams.Length > yParams.Length)
                    return 1;
                if (xParams.Length == yParams.Length)
                    return 1;
                return -1;
            }
        }

        public List<ConstructorInfo> SortConstructors(List<ConstructorInfo> constructors)
        {
            Requires.EnsureTrue(constructors.Count > 0, "No constructors were found!");
            if (constructors.Count == 1)
                return constructors;
            constructors.Sort(new ConstructorInfoComparer());
            return constructors;
        }
    }
}
