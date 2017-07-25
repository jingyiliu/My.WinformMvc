using System;

namespace My.WinformMvc.DataBinding
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class DataBindingAttribute : Attribute
    {
        /// <summary>
        /// Specify the control name to which the data source will be bound to.
        /// If this value is null or empty, then we will use the data source property
        /// name to match the control name.
        /// </summary>
        /// <remarks>For example, the name of [TextBox] in the [Form].</remarks>
        public string ControlName { get; set; }
        /// <summary>
        /// Specify the property name of the binding control to which the data source will be bound to.
        /// If this value is null or empty, then the default binding property of the control will be used.
        /// For example:
        /// To [TextBox], the default value is [Text]; 
        /// To [ComboBox], the default value is [SelectedText].
        /// </summary>
        public string PropertyName { get; set; }
    }
}