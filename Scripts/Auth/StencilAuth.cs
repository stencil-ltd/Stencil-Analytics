#if !EXCLUDE_FIREBASE

using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Init;
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
        
        static StencilAuth()
        {
            if (GameInit.FirebaseReady)
            {
                _Init();
            }
            else
            {
                GameInit.OnFirebaseInit += (sender, args) => _Init();
            }
        }

        private static void _Init()
        {
            if (!GameInit.FirebaseReady) return;
            IsReady = true;
            FirebaseAuth.DefaultInstance.IdTokenChanged += IdTokenChanged;
            IdTokenChanged?.Invoke(null, EventArgs.Empty);
        }

        public static Task<FirebaseUser> SignInAnonymouslyAsync()
        {
            if (GameInit.FirebaseReady)
                return FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync();
            return Task.FromException<FirebaseUser>(new Exception("Firebase not ready"));
        }
    }
}

#endif