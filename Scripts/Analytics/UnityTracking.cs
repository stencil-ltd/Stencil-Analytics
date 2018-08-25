using System.Collections.Generic;

namespace Analytics
{
    public class UnityTracking : ITracker
    {
        public ITracker Track(string name, Dictionary<string, object> eventData)
        {
            UnityEngine.Analytics.Analytics.CustomEvent(name, eventData);
            return this;
        }

        public ITracker SetUserProperty(string name, object value)
        {
            return this;
        }
    }
}