using System.Collections.Generic;
using JetBrains.Annotations;

namespace Analytics
{
    public interface ITracker
    {
        ITracker Track(string name, [CanBeNull] Dictionary<string, object> eventData);
        ITracker SetUserProperty(string name, object value);
    }
}