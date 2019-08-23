#if STENCIL_FIREBASE
using Firebase;

namespace Scripts.Firebase
{
    public class StencilFirebase
    {
        public static bool IsReady => FirebaseApp.DefaultInstance != null;
    }
}
#endif