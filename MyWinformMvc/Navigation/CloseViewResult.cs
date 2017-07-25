
namespace My.WinformMvc.Navigation
{
    class CloseViewResult : IActionResult
    {
        static readonly CloseViewResult _instance = new CloseViewResult();

        private CloseViewResult() { }

        internal static CloseViewResult Instance
        {
            get { return _instance; }
        }

        public void ExecuteResult(IController controller)
        {
            controller.Dispose();
        }
    }
}