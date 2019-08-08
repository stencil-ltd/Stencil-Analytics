using System.Collections.Generic;

namespace Analytics
{
    public interface ITrackingInterceptor
    {
        void ProcessArgs(Dictionary<string, object> args);
    }
}