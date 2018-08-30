using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dev;
using Debug = UnityEngine.Debug;

#if UNITY_FABRIC
using Fabric.Crashlytics;
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
            
            #if STENCIL_FIREBASE
            _trackers.Add(new FirebaseTracking());
            #endif
            
            #if STENCIL_FABRIC
            _trackers.Add(new FabricTracking());
            #endif
            
            #if STENCIL_FACEBOOK
            _trackers.Add(new FacebookTracking());
            #endif
        }

        public ITracker Track(string name, Dictionary<string, object> eventData = null)
        {
            if (!Enabled) return this;
            var json = eventData == null ? "[]" : string.Join(", ", eventData.ToList());
            Debug.Log($"Track Event {name}\n{json}");
            foreach (var tracker in _trackers) tracker.Track(name, eventData);
            return this;
        }

        public ITracker SetUserProperty(string name, object value)
        {
            if (!Enabled) return this;
            foreach (var tracker in _trackers) tracker.SetUserProperty(name, value);
            return this;
        }
        

        public static void Record(string message)
        {
            Debug.Log(message);
        }

        public static void Warn(string message)
        {
            var stacktrace = new StackTrace(1, true);
            Debug.LogWarning($"Warning: {message} - {stacktrace}");
            #if UNITY_FABRIC
            Crashlytics.RecordCustomException("Warning", message, stacktrace);
            #endif
        }
    }
}