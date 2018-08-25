#if STENCIL_FACEBOOK

using System.Collections.Generic;
using Facebook.Unity;

namespace Analytics
{
    public class FacebookTracking : ITracker
    {
        public ITracker Track(string name, Dictionary<string, object> eventData)
        {
            FB.LogAppEvent(name, null, eventData);
            return this;
        }

        public ITracker SetUserProperty(string name, object value)
        {
            // Nothing.
            return this;
        }
    }
}

#endif