using System;
using System.Collections.Generic;
using My.Helpers;
using My.IoC.Core;
using My.IoC.Lifetimes;

namespace My.IoC.Extensions.OpenGeneric
{
    /// <summary>
    /// Indicates how to select the constructor to be injected into and specifies the default defaultParameters.
    /// </summary>
    public interface IConstructorApi : IInjectorKindApi
    {
        /// <summary>
        /// Specify the constructor selector, which will be used along with the default defaultParameters (if any) 
        /// to select an eligible constructor to inject in.
        /// </summary>
        /// <param name="ctorSelector">The constructor selector.</param>
        /// <returns></returns>
        IInjectorKindApi WithConstructor(IConstructorSelector ctorSelector);

        /// <summary>
        /// Provides the default constructor defaultParameters, these defaultParameters will affect how an eligible constructor 
        /// to be injected into is selected, and will be reused when creating a object instance.
        /// </summary>
        /// <param name="ctorParameters">The default parameters.</param>
        /// <returns></returns>
        IInjectorKindApi WithConstructor(params PositionalParameter[] ctorParameters);
        /// <summary>
        /// Provides the default constructor defaultParameters, these defaultParameters will affect how an eligible constructor 
        /// to be injected into is selected, and will be reused when creating a object instance.
        /// </summary>
        /// <param name="ctorParameters">The default parameters.</param>
        /// <returns></returns>
        IInjectorKindApi WithConstructor(IEnumerable<PositionalParameter> ctorParameters);
        /// <summary>
        /// Provides the default constructor defaultParameters, these defaultParameters will affect how an eligible constructor 
        /// to be injected into is selected, and will be reused when creating a object instance.
        /// </summary>
        /// <param name="ctorParameters">The default parameters.</param>
        /// <returns></returns>
        IInjectorKindApi WithConstructor(IList<PositionalParameter> ctorParameters);
        /// <summary>
        /// Provides the default constructor defaultParameters and specify the constructor selector. These defaultParameters will 
        /// affect how an eligible constructor to be injected into is selected, and will be reused when creating a 
        /// object instance. The constructor selector will be used along with the default defaultParameters to select an 
        /// eligible constructor to inject in.
        /// </summary>
        /// <param name="ctorSelector">The constructor selector.</param>
        /// <param name="ctorParameters">The default constructor parameters.</param>
        /// <returns></returns>
        IInjectorKindApi WithConstructor(IConstructorSelector ctorSelector, params PositionalParameter[] ctorParameters);
        /// <summary>
        /// Provides the default constructor defaultParameters and specify the constructor selector. These defaultParameters will 
        /// affect how an eligible constructor to be injected into is selected, and will be reused when creating a 
        /// object instance. The constructor selector will be used along with the default defaultParameters to select an 
        /// eligible constructor to inject in.
        /// </summary>
        /// <param name="ctorSelector">The constructor selector.</param>
        /// <param name="ctorParameters">The default constructor parameters.</param>
        /// <returns></returns>
        IInjectorKindApi WithConstructor(IConstructorSelector ctorSelector, IEnumerable<PositionalParameter> ctorParameters);
        /// <summary>
        /// Provides the default constructor defaultParameters and specify the constructor selector. These defaultParameters will 
        /// affect how an eligible constructor to be injected into is selected, and will be reused when creating a 
        /// object instance. The constructor selector will be used along with the default defaultParameters to select an 
        /// eligible constructor to inject in.
        /// </summary>
        /// <param name="ctorSelector">The constructor selector.</param>
        /// <param name="ctorParameters">The default constructor parameters.</param>
        /// <returns></returns>
        IInjectorKindApi WithConstructor(IConstructorSelector ctorSelector, IList<PositionalParameter> ctorParameters);

        /// <summary>
        /// Provides the default constructor defaultParameters, these defaultParameters will affect how an eligible constructor 
        /// to be injected into is selected, and will be reused when creating a object instance.
        /// </summary>
        /// <param name="ctorParameters">The default parameters.</param>
        /// <returns></returns>
        IInjectorKindApi WithConstructor(params NamedParameter[] ctorParameters);
        /// <summary>
        /// Provides the default constructor defaultParameters, these defaultParameters will affect how an eligible constructor 
        /// to be injected into is selected, and will be reused when creating a object instance.
        /// </summary>
        /// <param name="ctorParameters">The default parameters.</param>
        /// <returns></returns>
        IInjectorKindApi WithConstructor(IEnumerable<NamedParameter> ctorParameters);
        /// <summary>
        /// Provides the default constructor defaultParameters, these defaultParameters will affect how an eligible constructor 
        /// to be injected into is selected, and will be reused when creating a object instance.
        /// </summary>
        /// <param name="ctorParameters">The default parameters.</param>
        /// <returns></returns>
        IInjectorKindApi WithConstructor(IList<NamedParameter> ctorParameters);
        /// <summary>
        /// Provides the default constructor defaultParameters and specify the constructor selector. These defaultParameters will 
        /// affect how an eligible constructor to be injected into is selected, and will be reused when creating a 
        /// object instance. The constructor selector will be used along with the default defaultParameters to select an 
        /// eligible constructor to inject in.
        /// </summary>
        /// <param name="ctorSelector">The constructor selector.</param>
        /// <param name="ctorParameters">The default constructor parameters.</param>
        /// <returns></returns>
        IInjectorKindApi WithConstructor(IConstructorSelector ctorSelector, params NamedParameter[] ctorParameters);
        /// <summary>
        /// Provides the default constructor defaultParameters and specify the constructor selector. These defaultParameters will 
        /// affect how an eligible constructor to be injected into is selected, and will be reused when creating a 
        /// object instance. The constructor selector will be used along with the default defaultParameters to select an 
        /// eligible constructor to inject in.
        /// </summary>
        /// <param name="ctorSelector">The constructor selector.</param>
        /// <param name="ctorParameters">The default constructor parameters.</param>
        /// <returns></returns>
        IInjectorKindApi WithConstructor(IConstructorSelector ctorSelector, IEnumerable<NamedParameter> ctorParameters);
        /// <summary>
        /// Provides the default constructor defaultParameters and specify the constructor selector. These defaultParameters will 
        /// affect how an eligible constructor to be injected into is selected, and will be reused when creating a 
        /// object instance. The constructor selector will be used along with the default defaultParameters to select an 
        /// eligible constructor to inject in.
        /// </summary>
        /// <param name="ctorSelector">The constructor selector.</param>
        /// <param name="ctorParameters">The default constructor parameters.</param>
        /// <returns></returns>
        IInjectorKindApi WithConstructor(IConstructorSelector ctorSelector, IList<NamedParameter> ctorParameters);
    }
    public interface IInjectorKindApi : IInApi
    {
        IInApi ActivationMode(ActivatorKind kind);
    }
    /// <summary>
    /// Provides a set of methods to specify the lifetime for which the created object instance will live in.
    /// </summary>
    public interface IInApi
    {
        /// <summary>
        /// Indicates the lifetime for which the created object instance will live in. See
        /// <see cref="Lifetime{T}"/> for more information.
        /// </summary>
        /// <param name="lifetimeProvider">The lifetime provider.</param>
        void In(ILifetimeProvider lifetimeProvider);
    }

    class OpenGenericConfigurationApi : IConstructorApi
    {
        readonly OpenGenericRegistrationData _registrationData = new OpenGenericRegistrationData();

        public void Register(Type openGenericContractType, Type openGenericConcreteType)
        {
            _registrationData.OpenGenericContractType = openGenericContractType;
            _registrationData.OpenGenericConcreteType = openGenericConcreteType;
        }

        public OpenGenericRegistrationData GetRegistrationData()
        {
            return _registrationData;
        }

        #region IConstructorApi Members

        public IInjectorKindApi WithConstructor(IConstructorSelector ctorSelector)
        {
            _registrationData.ConstructorSelector = ctorSelector;
            return this;
        }

        public IInjectorKindApi WithConstructor(params PositionalParameter[] ctorParameters)
        {
            _registrationData.ConfiguredParameters = new PositionalParameterSet(ctorParameters);
            return this;
        }

        public IInjectorKindApi WithConstructor(IEnumerable<PositionalParameter> ctorParameters)
        {
            _registrationData.ConfiguredParameters = new PositionalParameterSet(ctorParameters.ToList());
            return this;
        }

        public IInjectorKindApi WithConstructor(IList<PositionalParameter> ctorParameters)
        {
            _registrationData.ConfiguredParameters = new PositionalParameterSet(ctorParameters);
            return this;
        }

        public IInjectorKindApi WithConstructor(IConstructorSelector ctorSelector, params PositionalParameter[] ctorParameters)
        {
            _registrationData.ConstructorSelector = ctorSelector;
            _registrationData.ConfiguredParameters = new PositionalParameterSet(ctorParameters);
            return this;
        }

        public IInjectorKindApi WithConstructor(IConstructorSelector ctorSelector, IEnumerable<PositionalParameter> ctorParameters)
        {
            _registrationData.ConstructorSelector = ctorSelector;
            _registrationData.ConfiguredParameters = new PositionalParameterSet(ctorParameters.ToList());
            return this;
        }

        public IInjectorKindApi WithConstructor(IConstructorSelector ctorSelector, IList<PositionalParameter> ctorParameters)
        {
            _registrationData.ConstructorSelector = ctorSelector;
            _registrationData.ConfiguredParameters = new PositionalParameterSet(ctorParameters);
            return this;
        }

        public IInjectorKindApi WithConstructor(params NamedParameter[] ctorParameters)
        {
            _registrationData.ConfiguredParameters = new NamedParameterSet(ctorParameters);
            return this;
        }

        public IInjectorKindApi WithConstructor(IEnumerable<NamedParameter> ctorParameters)
        {
            _registrationData.ConfiguredParameters = new NamedParameterSet(ctorParameters.ToList());
            return this;
        }

        public IInjectorKindApi WithConstructor(IList<NamedParameter> ctorParameters)
        {
            _registrationData.ConfiguredParameters = new NamedParameterSet(ctorParameters);
            return this;
        }

        public IInjectorKindApi WithConstructor(IConstructorSelector ctorSelector, params NamedParameter[] ctorParameters)
        {
            _registrationData.ConstructorSelector = ctorSelector;
            _registrationData.ConfiguredParameters = new NamedParameterSet(ctorParameters);
            return this;
        }

        public IInjectorKindApi WithConstructor(IConstructorSelector ctorSelector, IEnumerable<NamedParameter> ctorParameters)
        {
            _registrationData.ConstructorSelector = ctorSelector;
            _registrationData.ConfiguredParameters = new NamedParameterSet(ctorParameters.ToList());
            return this;
        }

        public IInjectorKindApi WithConstructor(IConstructorSelector ctorSelector, IList<NamedParameter> ctorParameters)
        {
            _registrationData.ConstructorSelector = ctorSelector;
            _registrationData.ConfiguredParameters = new NamedParameterSet(ctorParameters);
            return this;
        }

        #endregion

        #region IInjectorKindApi Members

        public IInApi ActivationMode(ActivatorKind kind)
        {
            _registrationData.ActivatorKind = kind;
            return this;
        }

        #endregion

        #region IInApi Members

        public void In(ILifetimeProvider lifetimeProvider)
        {
            _registrationData.LifetimeProvider = lifetimeProvider;
        }

        #endregion
    }
}
