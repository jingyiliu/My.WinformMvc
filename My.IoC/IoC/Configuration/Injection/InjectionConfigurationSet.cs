
using System;
using System.Collections.Generic;
using My.IoC.Activities;
using My.IoC.Core;
using My.IoC.Dependencies;
using My.Helpers;

namespace My.IoC.Configuration.Injection
{
    //public interface IInjectionConfigurationSet
    //{
    //    ObjectDescription ObjectDescription { get; }
    //    InjectionConfigurationGroup DefaultInjectionConfigurationGroup { get; }

    //    // Aop/Decorator pattern, etc...
    //    IEnumerable<InjectionConfigurationGroup> CustomInjectionConfigurationGroups { get; }
    //    int CustomInjectionConfigurationGroupCount { get; }
    //    InjectionConfigurationGroup GetCustomInjectionConfigurationGroup(int index);
    //    void AddCustomInjectionConfigurationGroup(InjectionConfigurationGroup group);
    //    void InsertCustomInjectionConfigurationGroup(int index, InjectionConfigurationGroup group);
    //    void RemoveCustomInjectionConfigurationGroup(int index);
    //    void RemoveAllCustomInjectionConfigurationGroups();

    //    InjectionProcess<T> CreateInjectionProcess<T>(Kernel kernel);
    //}

    public class InjectionConfigurationSet //: IInjectionConfigurationSet
    {
        readonly ObjectDescription _description;
        readonly ObjectRelation _admin;
        readonly InjectionConfigurationGroup _defaultGroup;
        List<InjectionConfigurationGroup> _customGroups;

        public InjectionConfigurationSet(ObjectDescription description, ObjectRelation admin, InjectionConfigurationGroup defaultGroup)
        {
            if (!defaultGroup.MatchInjectionConfigurationSet(this))
                throw new InvalidOperationException();

            _description = description;
            _admin = admin;
            _defaultGroup = defaultGroup;
        }

        public ObjectRelation ObjectRelation
        {
            get { return _admin; }
        }

        public ObjectDescription ObjectDescription
        {
            get { return _description; }
        }

        public InjectionConfigurationGroup DefaultInjectionConfigurationGroup
        {
            get { return _defaultGroup; }
        }

        #region CustomInjectionConfigurationGroup Members

        public IEnumerable<InjectionConfigurationGroup> CustomInjectionConfigurationGroups
        {
            get { return _customGroups; }
        }

        public void AddCustomInjectionConfigurationGroup(InjectionConfigurationGroup group)
        {
            Requires.NotNull(group, "group");
            if (!group.MatchInjectionConfigurationSet(this))
                return;

            if (_customGroups == null)
                _customGroups = new List<InjectionConfigurationGroup>();
            _customGroups.Add(group);
        }

        #endregion

        internal InjectionProcess<T> CreateInjectionProcess<T>(Kernel kernel)
        {
            if (_defaultGroup == null)
                throw new InvalidOperationException("");

            List<DependencyProvider> dependencyProviders;
            var activity = _defaultGroup.CreateInjectionActivity<T>(kernel, out dependencyProviders);

            var process = new InjectionProcess<T>();
            process.AddActivity(activity);

            if (_customGroups == null)
            {
                _admin.BuildDependencyRelationship(dependencyProviders);
                return process;
            }

            for (int i = 0; i < _customGroups.Count; i++)
            {
                List<DependencyProvider> depProviders;
                var customActivity = _customGroups[i].CreateInjectionActivity<T>(kernel, out depProviders);
                if (depProviders != null)
                {
                    if (dependencyProviders == null)
                        dependencyProviders = depProviders;
                    else
                        dependencyProviders.AddRange(depProviders);
                }
                process.AddActivity(customActivity);
            }

            _admin.BuildDependencyRelationship(dependencyProviders);
            return process;
        }
    }
}

