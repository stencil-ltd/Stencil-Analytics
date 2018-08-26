#if STENCIL_FABRIC
using System.Collections.Generic;
using Fabric.Answers;
using Fabric.Crashlytics;
using Fabric.Internal.ThirdParty.MiniJSON;

namespace Analytics
{
    public class FabricTracking : ITracker
    {
        public FabricTracking()
        {
            
        }

        public ITracker Track(string name, Dictionary<string, object> eventData)
        {
            Answers.LogCustom(name, eventData);
            Crashlytics.Log($"{name}: {Json.Serialize(eventData)}");
            return this;
        }

        public ITracker SetUserProperty(string name, object value)
        {
            Crashlytics.Log($"Set Property {name} = {value}");
            Crashlytics.SetKeyValue(name, value?.ToString());
            return this;
        }
    }
}
#endif