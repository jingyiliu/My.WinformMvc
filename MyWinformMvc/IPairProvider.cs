using System.Collections.Generic;

namespace My.WinformMvc
{
    public interface IPairProvider
    {
        IPairRule PairRule { get; }
        IEnumerable<ViewControllerPair> ViewControllerPairs { get; }
        bool TryGetViewControllerPair(string pairName, out ViewControllerPair viewControllerPair);
    }
}