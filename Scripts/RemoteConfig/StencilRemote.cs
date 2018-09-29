using System;
using System.Collections.Generic;
using Dev;
using Firebase.RemoteConfig;
using Init;
using UnityEngine;
using Util;
using Versions;

namespace Scripts.RemoteConfig
{
    public static class StencilRemote
    {
        public static int CacheHours = 1;
        
        public static event EventHandler OnRemoteConfig;
        public static void NotifyRemoteConfig() => OnRemoteConfig?.Invoke();
        
        public static long GetProdVersion()
        {
            return GetValue("version", true)?.LongValue(long.MaxValue) ?? long.MaxValue;
        }
        
        public static bool IsProd()
        {
            if (Developers.Enabled) return false;
            var localVersion = VersionCodes.GetVersionCode();
            return localVersion <= GetProdVersion();
        }

        public static ConfigValue? GetValue(string key, bool forceProd = false)
        {
            var orig = key;
            if (!forceProd) key = key.Process();
            if (!GameInit.FirebaseReady) return null;
            var value = FirebaseRemoteConfig.GetValue(key);
            if (!IsProd() && !value.HasValue())
                value = FirebaseRemoteConfig.GetValue(orig);
            return value;
        }

        private static string Process(this string key)
        {
            if (Developers.Enabled || !IsProd()) return $"{key}_debug";
            return key;
        }
        
        public static bool HasValue(string key)
        {
            return GetValue(key.Process())?.HasValue() ?? false;
        }
    }
}