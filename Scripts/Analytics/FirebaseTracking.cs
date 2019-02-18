#if !EXCLUDE_FIREBASE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using Firebase;
using Firebase.Analytics;
using Init;

#if NEW_CRASHLYTICS
using Facebook.MiniJSON;
using Firebase.Crashlytics;
#endif

namespace Analytics
{
    public class FirebaseTracking : ITracker
    {
        public ITracker Track(string name, Dictionary<string, object> eventData)
        {
            if (!GameInit.FirebaseReady) return this;
            if (eventData == null)
            {
                FirebaseAnalytics.LogEvent(name);
            }
            else
            {
                FirebaseAnalytics.LogEvent(name, eventData.Select(kv =>
                {
                    var value = kv.Value;
                    if (value is double || value is float || value is decimal)
                        return new Parameter(kv.Key, Convert.ToDouble(kv.Value));
                    if (value is long || value is int || value is byte || value is bool)
                        return new Parameter(kv.Key, Convert.ToInt64(kv.Value));
                    if (value is ulong || value is uint)
                        return new Parameter(kv.Key, Convert.ToUInt64(kv.Value));
                    if (value is string || value is char || value is Enum)
                        return new Parameter(kv.Key, $"{kv.Value}");
                    throw new Exception($"Unrecognized tracking type for {kv.Key}: {value.GetType()}");
                }).ToArray());
            }

            #if NEW_CRASHLYTICS
            Crashlytics.Log($"{name}: {Json.Serialize(eventData)}");
            #endif
            return this;
        }

        public ITracker SetUserProperty(string name, object value)
        {
            if (!GameInit.FirebaseReady) return this;
            FirebaseAnalytics.SetUserProperty(name, value?.ToString());
            #if NEW_CRASHLYTICS
            Crashlytics.Log($"Set Property {name} = {value}");
            Crashlytics.SetKeyValue(name, value?.ToString());
            #endif
            return this;
        }
    }
}
#endif