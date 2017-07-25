using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace My.WinformMvc.DataBinding
{
    class DataBindingInfo
    {
        // PropertyInfo 上是否定义了自定义属性。每种控件都有默认的绑定属性，例如Textbox的默认属性为Text。但DataSource可以在自己的Property上使用Attribute指定要绑定控件的属性名（例如Tag）。
        // 绑定时，还需检查控件的属性可以接受的数据类型是否与数据源的属性所提供的数据类型兼容。例如 Number 控件只能接受数字，此时数据源如果不能转成数据则不行。此时可能会有数据转换或者格式化。
        // 如果数据源属性上没有自定义属性，则将绑定所对应控件的默认属性。
        public string ControlName { get; set; } // 例如 Form 中定义的某个控件（例如 TextBox）的 Name

        public string PropertyName { get; set; } // 例如上述 TextBox 的某个属性（例如 Text 或 Tag）的属性名
        public Type PropertyType { get; set; } // 例如上述属性（例如 Text 或 Tag）的类型，该属性主要为了类型转换。例如数据源的类型是 string，但此处属性的类型是 int，那么则需要类型转换

        public PropertyInfo DataProperty { get; set; }
        public string DataMember
        {
            get { return DataProperty.Name; }
        }
    }

    class BindingFactory
    {
        static readonly Type _baseControlType = typeof(Control);
        // 例如 TextBox 对应 "Text" 的 PropertyInfo
        readonly Dictionary<Type, PropertyInfo> _control2DafaultBindingProperties = new Dictionary<Type, PropertyInfo>();

        internal BindingFactory()
        {
            AddDefaultMapping(typeof (TextBox), "Text");
            AddDefaultMapping(typeof (ComboBox), "SelectedText");
        }

        internal void AddDefaultMapping(Type controlType, string propertyName)
        {
            var property = controlType.GetProperty(propertyName);
            _control2DafaultBindingProperties.Add(controlType, property);
        }

        internal DataBinder GetDataBinding(Type containerType, Type dataSourceType, string suffix)
        {
            var bindingInfos = new List<DataBindingInfo>();
            // Get the private fields of the [Form] by reflection, will this require permission in restricted enviroment?
            var containerFields = containerType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            var dataProperties = dataSourceType.GetProperties(); // data source properties

            foreach (var dataProperty in dataProperties)
            {
                var attribs = dataProperty.GetCustomAttributes(typeof (DataBindingAttribute), false);
                if (attribs.Length != 1) // If the data source property is not bindable
                    continue; 

                var attrib = (DataBindingAttribute)attribs[0];
                var controlName = string.IsNullOrEmpty(attrib.ControlName)
                    ? dataProperty.Name + (suffix ?? string.Empty)
                    : attrib.ControlName;

                var controlType = GetTargetControlType(containerFields, controlName); // Find the control
                if (controlType == null)
                    continue; // If the control can not be found, then we will just skip the data source property.

                // Bind to which property of the control, for example, the [Text] or [Tag]
                var property = string.IsNullOrEmpty(attrib.PropertyName) 
                    ? _control2DafaultBindingProperties[controlType]
                    : GetTargetControlProperty(controlType, attrib.PropertyName);

                var bindingInfo = new DataBindingInfo
                {
                    ControlName = controlName,
                    DataProperty = dataProperty,
                    PropertyType = property.PropertyType,
                    PropertyName = property.Name
                };
                bindingInfos.Add(bindingInfo);
            }

            return new DataBinder(bindingInfos);
        }

        static Type GetTargetControlType(FieldInfo[] containerFields, string controlName)
        {
            foreach (var field in containerFields)
            {
                if (controlName.Equals(field.Name) && _baseControlType.IsAssignableFrom(field.FieldType))
                    return field.FieldType;
            }
            return null;
        }

        /// <summary>
        /// Gets the target control property.
        /// </summary>
        /// <param name="controlType">TextBox, ComboBox, etc</param>
        /// <param name="definedPropertyName">TextBox or Tag, etc</param>
        /// <returns></returns>
        static PropertyInfo GetTargetControlProperty(Type controlType, string definedPropertyName)
        {
            var controlProperties = controlType.GetProperties();
            foreach (var controlProperty in controlProperties)
            {
                if (definedPropertyName.Equals(controlProperty.Name))
                    return controlProperty;
            }
            throw new Exception(string.Format("Invalid property name [{0}] for control [{1}]!", definedPropertyName, controlType.FullName));
        }
    }

    /// <summary>
    /// Form (=) Model two way binding
    /// </summary>
    class DataBinder
    {
        readonly List<DataBindingInfo> _bindingInfos;

        internal DataBinder(List<DataBindingInfo> bindingInfos)
        {
            _bindingInfos = bindingInfos;
        }

        internal void Bind(Control control, object dataSource)
        {
            // Binding b = new Binding("controlPropertyName", dataSource, "dataMember", 
            //    formattingEnabled, 
            //    DataSourceUpdateMode.OnValidation, objectSetOnControlWhenDataSourceIsNull,
            //    formatString, IFormatProvider);
            foreach (var bindingInfo in _bindingInfos)
            {
                var targetControl = GetTargetControl(control, bindingInfo.ControlName);
                var binding = new Binding(bindingInfo.PropertyName, dataSource, bindingInfo.DataMember);
                targetControl.DataBindings.Add(binding);
            }
        }

        static Control GetTargetControl(Control container, string controlName)
        {
            if (controlName.Equals(container.Name))
                return container;
            
            foreach (Control control in container.Controls)
            {
                if (controlName.Equals(control.Name))
                    return control;
                var result = GetTargetControl(control, controlName);
                if (result != null)
                    return result;
            }

            return null;
        }
    }

    public class DataBindingManager
    {
        struct Key
        {
            public Type ControlType { get; set; }
            public Type DataSourceType { get; set; }
        }

        readonly BindingFactory _factory = new BindingFactory();
        readonly Dictionary<Key, DataBinder> _mappings = new Dictionary<Key, DataBinder>();

        public void AddDefaultMapping(Type controlType, string propertyName)
        {
            _factory.AddDefaultMapping(controlType, propertyName);
        }

        public void BindDataSource(Control control, object dataSource, string suffix)
        {
            var key = new Key()
            {
                ControlType = control.GetType(),
                DataSourceType = dataSource.GetType()
            };
            DataBinder binder;
            if (!_mappings.TryGetValue(key, out binder))
            {
                binder = _factory.GetDataBinding(key.ControlType, key.DataSourceType, suffix);
                _mappings.Add(key, binder);
            }
            binder.Bind(control, dataSource);
        }
    }
}
