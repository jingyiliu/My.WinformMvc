using System.Reflection;

namespace My.WinformMvc.Filters.Contexts
{
    public abstract class FilterContext
    {
        MethodInfo _actionMethod;

        internal MethodInfo ActionMethod
        {
            set { _actionMethod = value; }
        }
        public string ActionName 
        {
            get { return _actionMethod == null ? null : _actionMethod.Name; }
        }
        public ICustomAttributeProvider ActionAttributeProvider
        {
            get { return _actionMethod; }
        }

        public object[] ActionParameters { get; internal set; }
        public BaseController Controller { get; internal set; }
        public Session Session
        {
            get { return Controller == null ? null : Controller.Session; }
        }

        public virtual IActionResult Result { get; set; }
    }
}