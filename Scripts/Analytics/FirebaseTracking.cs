#if STENCIL_FIREBASE
using System;
using System.Collections.Generic;
using System.Linq;
using Analytics;
using Firebase.Analytics;

namespace Analytics
{
    public class FirebaseTracking : ITracker
    {
        public ITracker Track(string name, Dictionary<string, object> eventData)
        {
            if (eventData == null)
            {
                FirebaseAnalytics.LogEvent(name);
            }
            else
            {
                FirebaseAnalytics.LogEvent(name, eventData.Select(kv =>
                {
                    var value = kv.Value;
                    if (value is double || value is float)
                        return new Parameter(kv.Key, Convert.ToDouble(kv.Value));
                    if (value is long || value is int || value is byte || value is bool)
                        return new Parameter(kv.Key, Convert.ToInt64(kv.Value));
                    if (value is string || value is char)
                        return new Parameter(kv.Key, $"{kv.Value}");
                    throw new Exception($"Unrecognized tracking type for {kv.Key}: {value.GetType()}");
                }).ToArray());
            }

            return this;
        }

        public ITracker SetUserProperty(string name, object value)
        {
            FirebaseAnalytics.SetUserProperty(name, value?.ToString());
            return this;
        }
    }
}
#endif