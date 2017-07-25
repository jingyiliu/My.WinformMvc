using System;

namespace My.WinformMvc
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class MvcControllerAttribute : Attribute
    {
        public MvcControllerAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("The controller name can not be null or empty!");
            Name = name;
        }

        public string Name { get; private set; }
    }
}
