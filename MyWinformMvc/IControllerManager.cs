
namespace My.WinformMvc
{
    public interface IControllerManager
    {
        IPairProvider PairProvider { get; }
        void RemoveController(IController controller, string pairName);
        BaseController GetOrCreateController(IController sourceController, string targetPairName);
        //BaseController CreateController(string pairName);
        //bool TryGetController(string pairName, out BaseController controller);
    }
}