
using System.Reflection;
using My.Foundation;
using My.IoC.Core;
using System.Collections.Generic;
using My.IoC.Injection.Func;

namespace My.IoC.Configuration.FluentApi
{
    public interface ITypeConfigurationApi : IConstructorApi
    {
    }



    /// <summary>
    /// Indicates how to select the constructor to be injected into and specifies the default defaultParameters.
    /// </summary>
    public interface IConstructorApi : IMemberApi
    {
        /// <summary>
        /// Specify the constructor selector, which will be used along with the default defaultParameters (if any) 
        /// to select an eligible constructor to inject in.
        /// </summary>
        /// <param name="ctorSelector">The constructor selector.</param>
        /// <returns></returns>
        IMemberApi WithConstructor(IConstructorSelector ctorSelector);

        /// <summary>
        /// Provides the default constructor defaultParameters, these defaultParameters will affect how an eligible constructor 
        /// to be injected into is selected, and will be reused when creating a object instance.
        /// </summary>
        /// <param name="ctorParameters">The default parameters.</param>
        /// <returns></returns>
        IMemberApi WithConstructor(params PositionalParameter[] ctorParameters);
        /// <summary>
        /// Provides the default constructor defaultParameters, these defaultParameters will affect how an eligible constructor 
        /// to be injected into is selected, and will be reused when creating a object instance.
        /// </summary>
        /// <param name="ctorParameters">The default parameters.</param>
        /// <returns></returns>
        IMemberApi WithConstructor(IEnumerable<PositionalParameter> ctorParameters);
        /// <summary>
        /// Provides the default constructor defaultParameters, these defaultParameters will affect how an eligible constructor 
        /// to be injected into is selected, and will be reused when creating a object instance.
        /// </summary>
        /// <param name="ctorParameters">The default parameters.</param>
        /// <returns></returns>
        IMemberApi WithConstructor(IList<PositionalParameter> ctorParameters);
        /// <summary>
        /// Provides the default constructor defaultParameters and specify the constructor selector. These defaultParameters will 
        /// affect how an eligible constructor to be injected into is selected, and will be reused when creating a 
        /// object instance. The constructor selector will be used along with the default defaultParameters to select an 
        /// eligible constructor to inject in.
        /// </summary>
        /// <param name="ctorSelector">The constructor selector.</param>
        /// <param name="ctorParameters">The default constructor parameters.</param>
        /// <returns></returns>
        IMemberApi WithConstructor(IConstructorSelector ctorSelector, params PositionalParameter[] ctorParameters);
        /// <summary>
        /// Provides the default constructor defaultParameters and specify the constructor selector. These defaultParameters will 
        /// affect how an eligible constructor to be injected into is selected, and will be reused when creating a 
        /// object instance. The constructor selector will be used along with the default defaultParameters to select an 
        /// eligible constructor to inject in.
        /// </summary>
        /// <param name="ctorSelector">The constructor selector.</param>
        /// <param name="ctorParameters">The default constructor parameters.</param>
        /// <returns></returns>
        IMemberApi WithConstructor(IConstructorSelector ctorSelector, IEnumerable<PositionalParameter> ctorParameters);
        /// <summary>
        /// Provides the default constructor defaultParameters and specify the constructor selector. These defaultParameters will 
        /// affect how an eligible constructor to be injected into is selected, and will be reused when creating a 
        /// object instance. The constructor selector will be used along with the default defaultParameters to select an 
        /// eligible constructor to inject in.
        /// </summary>
        /// <param name="ctorSelector">The constructor selector.</param>
        /// <param name="ctorParameters">The default constructor parameters.</param>
        /// <returns></returns>
        IMemberApi WithConstructor(IConstructorSelector ctorSelector, IList<PositionalParameter> ctorParameters);

        /// <summary>
        /// Provides the default constructor defaultParameters, these defaultParameters will affect how an eligible constructor 
        /// to be injected into is selected, and will be reused when creating a object instance.
        /// </summary>
        /// <param name="ctorParameters">The default parameters.</param>
        /// <returns></returns>
        IMemberApi WithConstructor(params NamedParameter[] ctorParameters);
        /// <summary>
        /// Provides the default constructor defaultParameters, these defaultParameters will affect how an eligible constructor 
        /// to be injected into is selected, and will be reused when creating a object instance.
        /// </summary>
        /// <param name="ctorParameters">The default parameters.</param>
        /// <returns></returns>
        IMemberApi WithConstructor(IEnumerable<NamedParameter> ctorParameters);
        /// <summary>
        /// Provides the default constructor defaultParameters, these defaultParameters will affect how an eligible constructor 
        /// to be injected into is selected, and will be reused when creating a object instance.
        /// </summary>
        /// <param name="ctorParameters">The default parameters.</param>
        /// <returns></returns>
        IMemberApi WithConstructor(IList<NamedParameter> ctorParameters);
        /// <summary>
        /// Provides the default constructor defaultParameters and specify the constructor selector. These defaultParameters will 
        /// affect how an eligible constructor to be injected into is selected, and will be reused when creating a 
        /// object instance. The constructor selector will be used along with the default defaultParameters to select an 
        /// eligible constructor to inject in.
        /// </summary>
        /// <param name="ctorSelector">The constructor selector.</param>
        /// <param name="ctorParameters">The default constructor parameters.</param>
        /// <returns></returns>
        IMemberApi WithConstructor(IConstructorSelector ctorSelector, params NamedParameter[] ctorParameters);
        /// <summary>
        /// Provides the default constructor defaultParameters and specify the constructor selector. These defaultParameters will 
        /// affect how an eligible constructor to be injected into is selected, and will be reused when creating a 
        /// object instance. The constructor selector will be used along with the default defaultParameters to select an 
        /// eligible constructor to inject in.
        /// </summary>
        /// <param name="ctorSelector">The constructor selector.</param>
        /// <param name="ctorParameters">The default constructor parameters.</param>
        /// <returns></returns>
        IMemberApi WithConstructor(IConstructorSelector ctorSelector, IEnumerable<NamedParameter> ctorParameters);
        /// <summary>
        /// Provides the default constructor defaultParameters and specify the constructor selector. These defaultParameters will 
        /// affect how an eligible constructor to be injected into is selected, and will be reused when creating a 
        /// object instance. The constructor selector will be used along with the default defaultParameters to select an 
        /// eligible constructor to inject in.
        /// </summary>
        /// <param name="ctorSelector">The constructor selector.</param>
        /// <param name="ctorParameters">The default constructor parameters.</param>
        /// <returns></returns>
        IMemberApi WithConstructor(IConstructorSelector ctorSelector, IList<NamedParameter> ctorParameters);
    }

    public interface IMemberApi : IActivatorKindApi
    {
        IMemberApi WithPropertiesAutowired();
        IMemberApi WithPropertyAutowired(string propertyName);
        IMemberApi WithPropertyAutowired(PropertyInfo property);

        IMemberApi WithPropertyValue<TProperty>(string propertyName, TProperty propertyValue);
        IMemberApi WithPropertyValue(string propertyName, object propertyValue);
        IMemberApi WithPropertyValue<TProperty>(string propertyName, Func<IResolutionContext, TProperty> valueFactory);

        IMemberApi WithMethod(string methodName);
        IMemberApi WithMethod(MethodInfo method);
    }

    public interface IActivatorKindApi : ICommonConfigurationApi
    {
        ICommonConfigurationApi WithActivator(ActivatorKind kind);
        //ICommonConfigurationApi UseDynamicInjector();
        //ICommonConfigurationApi UseReflectionInjector();
    }
}
