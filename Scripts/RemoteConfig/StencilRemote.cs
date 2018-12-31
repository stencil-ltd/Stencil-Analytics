using System;
using System.Collections.Generic;
using Init;
using UnityEngine;
using Util;
using Versions;
using Developers = Dev.Developers;
#if !EXCLUDE_FIREBASE
using Firebase.RemoteConfig;
#endif

namespace Scripts.RemoteConfig
{
    public static class StencilRemote
    {
        public static event EventHandler OnRemoteConfig;
        public static void NotifyRemoteConfig() => OnRemoteConfig?.Invoke();
        
        public static bool IsDeveloper() => !IsProd(); // because I'm an idiot who can't think
#if EXCLUDE_FIREBASE

        public static bool IsProd() => true;
        
#else
        public static int CacheHours = 1;

        private static bool HasBeenProd
        {
            get { return PlayerPrefsX.GetBool("stencil_been_prod"); }
            set { PlayerPrefsX.SetBool("stencil_been_prod", value); }
        }
        
        public static ConfigValue? GetProdVersionValue()
        {
            if (!GameInit.FirebaseReady) return null;
            return FirebaseRemoteConfig.GetValue("version");
        }
        
        public static long GetProdVersion()
        {
            if (!GameInit.FirebaseReady) return long.MaxValue;
            return FirebaseRemoteConfig.GetValue("version").LongValue(long.MaxValue);
        }

        public static bool IsProd()
        {
            if (Developers.Enabled) return false;
            if (!GameInit.FirebaseReady)
            {
                Debug.Log("Prod Check Skipped: Not ready.");
                return true;
            }
            if (HasBeenProd)
            {
                Debug.Log("Prod Check: This is a prod device forever");
                return true;
            }
            var localVersion = VersionCodes.GetVersionCode();
            var prodVersion = GetProdVersionValue();
            if (prodVersion == null || !prodVersion.Value.HasValue())
                return true;

            var version = prodVersion.Value.LongValue;
            Debug.Log($"Prod Check: {localVersion} -> {version}");
            var retval = localVersion <= version;
            if (retval) HasBeenProd = true;
            return retval;
        }

        public static ConfigValue GetValue(string key, bool forceProd = false)
        {
            var orig = key;
            if (!forceProd) key = key.Process();
            if (!GameInit.FirebaseReady) return new ConfigValue();
            var value = FirebaseRemoteConfig.GetValue(key);
            if (!value.HasValue() && !IsProd())
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
            return GetValue(key.Process()).HasValue();
        }
        
        public static long LongValue(string key, long defaultValue = default(long))
        {
            return GetValue(key.Process()).LongValue(defaultValue);
        }
                 
        public static int IntValue(string key, int defaultValue = default(int))
        {
            return GetValue(key.Process()).IntValue(defaultValue);
        }
         
        public static string StringValue(string key, string defaultValue = default(string))
        {
            return GetValue(key.Process()).StringValue;
        }
                 
        public static double DoubleValue(string key, double defaultValue = default(double))
        {
            return GetValue(key.Process()).DoubleValue(defaultValue);
        }
                 
        public static float FloatValue(string key, float defaultValue = default(float))
        {
            return GetValue(key.Process()).FloatValue(defaultValue);
        }
                 
        public static bool BoolValue(string key, bool defaultValue = default(bool))
        {
            return GetValue(key.Process()).BoolValue(defaultValue);
        }
                 
        public static IEnumerable<byte> ByteArrayValue(string key, IEnumerable<byte> defaultValue = null)
        {
            return GetValue(key.Process()).ByteArrayValue(defaultValue);
        }
    #endif
    }
}