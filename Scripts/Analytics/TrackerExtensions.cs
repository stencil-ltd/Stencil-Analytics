using System.Collections.Generic;

namespace Analytics
{
    public static class TrackerExtensions
    {
        public static ITracker Track(this ITracker tracker, string name, params object[] args)
        {
            var dict = new Dictionary<string, object>();
            for (var i = 0; i < args.Length; i += 2)
            {
                var key = (string) args[i];
                var value = args[i + 1];
                dict[key] = value;
            }
            tracker.Track(name, dict);
            return tracker;
        }
    }
}