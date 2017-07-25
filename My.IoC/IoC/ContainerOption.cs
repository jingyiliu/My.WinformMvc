
using System.Reflection;
using My.IoC.Core;

namespace My.IoC
{
    /// <summary>
    /// Contains configuration options for the <b>My.IoC</b>.
    /// </summary>
    public class ContainerOption
    {
        readonly bool _useLightweightCodeGeneration;
        readonly IConstructorSelector _constructorSelector;
        readonly IAutoRegistrationPolicy[] _autoRegistrationPolicies; 

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerOption"/> class.
        /// </summary>
        /// <param name="useLightweightCodeGeneration">if set to <c>true</c>, use lightweight code generation to create object instances.
        /// Otherwise, use the <see cref="System.Reflection"/> instead.</param>
        public ContainerOption(bool useLightweightCodeGeneration)
            : this(useLightweightCodeGeneration, (IConstructorSelector)null, null)
        {
        }

        public ContainerOption(bool useLightweightCodeGeneration, params IAutoRegistrationPolicy[] autoRegistrationPolicies)
            : this(useLightweightCodeGeneration, null, autoRegistrationPolicies)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerOption"/> class.
        /// </summary>
        /// <param name="useLightweightCodeGeneration">if set to <c>true</c>, use lightweight code generation to create object instances.
        /// Otherwise, use the <see cref="System.Reflection"/> instead.</param>
        /// <param name="ctorSelector">The constructor selector used to select an eligible constructor based on the provided parameter type and position.</param>
        public ContainerOption(bool useLightweightCodeGeneration, IConstructorSelector ctorSelector)
            : this(useLightweightCodeGeneration, ctorSelector, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerOption"/> class.
        /// </summary>
        /// <param name="useLightweightCodeGeneration">if set to <c>true</c>, use lightweight code generation to create object instances.
        /// Otherwise, use the <see cref="System.Reflection"/> instead.</param>
        /// <param name="ctorSelector">The constructor selector used to select an eligible constructor based on the provided parameter type and position.</param>
        /// <param name="autoRegistrationPolicies">The auto registration policies.</param>
        private ContainerOption(bool useLightweightCodeGeneration, IConstructorSelector ctorSelector, params IAutoRegistrationPolicy[] autoRegistrationPolicies)
        {
            _useLightweightCodeGeneration = useLightweightCodeGeneration;
            _constructorSelector = ctorSelector ?? new DefaultConstructorSelector(
                new BindingFlagsConstructorFinder(BindingFlags.Public),
                new AscendingConstructorSorter());
            _autoRegistrationPolicies = autoRegistrationPolicies;
        }

        /// <summary>
        /// Gets a value indicating whether <b>My.IoC</b> should use (usually faster) lightweight code 
        /// generation technique to create object instances.
        /// </summary>
        public bool UseLightweightCodeGeneration
        {
            get { return _useLightweightCodeGeneration; }
        }

        //public int MaxConstructorParameterLength
        //{
        //    get { return 32; }
        //}

        /// <summary>
        /// Gets the constructor selector used to select a eligible constructor to be injected into based 
        /// on the type and position of provided parameters.
        /// </summary>
        public IConstructorSelector ConstructorSelector
        {
            get { return _constructorSelector; }
        }

        /// <summary>
        /// Gets the constructor sorter used to sort the found constructors.
        /// </summary>
        public IConstructorSorter ConstructorSorter
        {
            get { return _constructorSelector.ConstructorSorter; }
        }

        /// <summary>
        /// Gets the constructor finder used to find suitable constructors from a concrete type.
        /// </summary>
        public IConstructorFinder ConstructorFinder
        {
            get { return _constructorSelector.ConstructorFinder; }
        }

        /// <summary>
        /// Gets the auto registration policies that will be applied when register concrete type 
        /// to the registry.
        /// </summary>
        public IAutoRegistrationPolicy[] AutoRegistrationPolicies
        {
            get { return _autoRegistrationPolicies; }
        }

        //public Action<Exception> Error = (e) => { };
    }
}
