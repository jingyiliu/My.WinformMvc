
using System.Collections.Generic;
using My.IoC.Core;

namespace My.IoC.Activities
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CompositeInjectionActivity<T> : InjectionActivity<T>
    {
        readonly List<InjectionActivity<T>> _activities = new List<InjectionActivity<T>>();

        public override void Execute(InjectionContext<T> context)
        {
            foreach (var activity in _activities)
                activity.Execute(context);
        }

        public void AddActivity(InjectionActivity<T> activity)
        {
            _activities.Add(activity);
        }
    }
}
