using UnityEngine;

#if !EXCLUDE_FABRIC
using Fabric.Internal;
using Fabric.Internal.Crashlytics;
#endif

namespace Analytics
{
    public class FabricHack : MonoBehaviour
    {
        #if !EXCLUDE_FABRIC
        private void Start()
        {
            new GameObject("Fabric Init").AddComponent<FabricInit>();
            new GameObject("Crashlytics Init").AddComponent<CrashlyticsInit>();
        }
        #endif
    }
}