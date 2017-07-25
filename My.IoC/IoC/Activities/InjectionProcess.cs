
using My.IoC.Core;

namespace My.IoC.Activities
{
    public class InjectionProcess<T>
    {
        InjectionActivity<T> _activity;

        public void Execute(InjectionContext<T> context)
        {
            _activity.Execute(context);
        }

        public void AddActivity(InjectionActivity<T> activity)
        {
            if (_activity == null)
            {
                _activity = activity;
                return;
            }

            var activities = _activity as CompositeInjectionActivity<T>;
            if (activities == null)
            {
                activities = new CompositeInjectionActivity<T>();
                activities.AddActivity(_activity);
            }
            activities.AddActivity(activity);
            _activity = activities;
        }
    }
}
