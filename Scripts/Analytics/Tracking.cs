using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Dev;
using JetBrains.Annotations;
using Debug = UnityEngine.Debug;

#if !EXCLUDE_FABRIC && !NEW_CRASHLYTICS
using Fabric.Crashlytics;
#endif

#if NEW_CRASHLYTICS
using Firebase.Crashlytics;
#endif

namespace Analytics
{
    public class Tracking : ITracker
    {
        public static readonly Tracking Instance = new Tracking(); 

        public bool Enabled => !Developers.Enabled;

        private readonly List<ITracker> _trackers = new List<ITracker>();

        private Tracking()
        {
            _trackers.Add(new UnityTracking());
            
            #if !EXCLUDE_FIREBASE
            _trackers.Add(new FirebaseTracking());
            #endif
            
            #if !EXCLUDE_FABRIC && !NEW_CRASHLYTICS
            _trackers.Add(new FabricTracking());
            #endif
            
            #if !EXCLUDE_FACEBOOK
            _trackers.Add(new FacebookTracking());
            #endif
        }

        public ITracker Track(string name, Dictionary<string, object> eventData = null)
        {
            name = Sanitize(name);
            eventData = Sanitize(eventData);
            
            var json = eventData == null ? "[]" : string.Join(", ", eventData.ToList());
            Debug.Log($"Track Event: {name}\n{json}");
            if (Enabled)
                foreach (var tracker in _trackers)
                    tracker.Track(name, eventData);
            return this;
        }

        public ITracker SetUserProperty(string name, object value)
        {
            name = Sanitize(name);
            value = SanitizeObject(value);
            Debug.Log($"User Property: {name} = {value}");
            if (Enabled)
                foreach (var tracker in _trackers)
                    tracker.SetUserProperty(name, value);
            return this;
        }

        public static void Report(string name, string reason, string stackTraceString)
        {
            #if !EXCLUDE_FABRIC
                Crashlytics.RecordCustomException(name, reason, stackTraceString);
            #else
                Debug.LogError($"{name}: {reason}");
            #endif
        }

        public static void Record(string message)
        {
            Debug.Log(message);
        }

        public static void Warn(string message)
        {
            var stacktrace = new StackTrace(1, true);
            Debug.LogWarning($"Warning: {message} - {stacktrace}");
            #if !EXCLUDE_FABRIC || NEW_CRASHLYTICS
            Crashlytics.RecordCustomException("Warning", message, stacktrace);
            #endif
        }

        public static void WarnIfNull(object thing, 
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string name = "")
        {
            if (thing == null)
                Warn($"[{line}] Null Reference: {name}");
        }

        private static string Sanitize([CanBeNull] string name)
        {
            return name?.Replace(".", "_").Replace(" ", "_").Replace("-", "_") ?? "";
        }

        private static object SanitizeObject([CanBeNull] object obj)
        {
            if (obj == null) return null;
            if (obj is string) obj = Sanitize((string) obj);
            return obj;
        }

        [CanBeNull]
        private static Dictionary<string, object> Sanitize([CanBeNull] Dictionary<string, object> eventData)
        {
            if (eventData == null) return null;
            var dict = new Dictionary<string, object>();
            foreach (var kv in eventData)
            {
                var key = Sanitize(kv.Key);
                var val = SanitizeObject(kv.Value);
                dict[key] = val;
            }
            return dict;
        }
    }
}