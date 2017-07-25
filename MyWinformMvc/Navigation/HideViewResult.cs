
namespace My.WinformMvc.Navigation
{
    class HideViewResult : IActionResult
    {
        static readonly HideViewResult _instance = new HideViewResult();

        private HideViewResult() { }

        internal static HideViewResult Instance
        {
            get { return _instance; }
        }

        public void ExecuteResult(IController controller)
        {
            if (controller.View.Visible)
                controller.View.Hide();
        }
    }
}