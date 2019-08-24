#if STENCIL_FIREBASE
using System;
using Firebase.Messaging;
using Firebase.RemoteConfig;
using Scripts.Auth;
using Scripts.RemoteConfig;
using Firebase;
using Scripts.Build;
using UI;
using UniRx.Async;
using UnityEngine;

namespace Stencil.Analytics.Firebase
{
    public class StencilFirebase : Controller<StencilFirebase>
    {
        public static bool IsReady { get; private set; }
        public static event EventHandler<bool> OnReady;
        
        private static UniTask? _init;

        private void Start()
        {
            var _ = _Init();
        }

        private static async UniTask _Init()
        {
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            var success = dependencyStatus == DependencyStatus.Available;
            if (!success)
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            IsReady = success;

            await UniTask.SwitchToMainThread();
            
            Debug.Log($"Firebase Configuration: {success}");
            if (success)
            {
                var prod = StencilRemote.IsProd();
                if (!prod) FirebaseApp.LogLevel = LogLevel.Debug;
                var settings = FirebaseRemoteConfig.Settings;
                settings.IsDeveloperMode = !prod;
                FirebaseRemoteConfig.Settings = settings;
                var cache = settings.IsDeveloperMode ? TimeSpan.Zero : TimeSpan.FromHours(StencilRemote.CacheHours);
                var _ = OnRemote(cache);
                SetupPush();
            }
            StencilAuth.Init();
            OnReady?.Invoke(null, success);
        }
        
        private static void SetupPush()
        {
            FirebaseMessaging.RequestPermissionAsync();
            if (StencilRemote.IsDeveloper())
                FirebaseMessaging.SubscribeAsync("dev/remote");
            else 
                FirebaseMessaging.UnsubscribeAsync("dev/remote");
            FirebaseMessaging.TokenReceived += (sender, args) =>
            {
                Debug.Log($"Firebase/FCM Token: {args.Token}");    
            };
        }

        private static async UniTask OnRemote(TimeSpan cache)
        {
            try
            {
                await FirebaseRemoteConfig.FetchAsync(cache);
                FirebaseRemoteConfig.ActivateFetched();
                await UniTask.SwitchToMainThread();
                StencilRemote.NotifyRemoteConfig();
            }
            catch (Exception e)
            {
                Debug.LogError($"Firebase Remote Config failed. {e.InnerException?.Message}");
            }
        }
    }
}
#endif