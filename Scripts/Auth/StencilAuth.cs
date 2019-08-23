#if STENCIL_FIREBASE

using System;
using System.Threading.Tasks;
using Firebase.Auth;
using JetBrains.Annotations;

namespace Scripts.Auth
{
    public static class StencilAuth
    {
        public static bool IsReady { get; private set; }

        [CanBeNull] public static FirebaseAuth Auth 
            => IsReady ? FirebaseAuth.DefaultInstance : null;

        [CanBeNull]
        public static FirebaseUser CurrentUser
            => Auth?.CurrentUser;

        public static event EventHandler IdTokenChanged;
        
        public static void Init()
        {
            IsReady = true;
            FirebaseAuth.DefaultInstance.IdTokenChanged += IdTokenChanged;
            IdTokenChanged?.Invoke(null, EventArgs.Empty);
        }

        public static Task<FirebaseUser> SignInAnonymouslyAsync()
        {
            if (IsReady)
                return FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync();
            return Task.FromException<FirebaseUser>(new Exception("Firebase not ready"));
        }
    }
}

#endif