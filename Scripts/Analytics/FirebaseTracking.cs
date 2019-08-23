﻿#if STENCIL_FIREBASE
using Dirichlet.Numerics;
using Firebase.Crashlytics;
using System;
using System.Collections.Generic;
using System.Linq;
using Firebase;
using Firebase.Analytics;
using Scripts.Firebase;
using Stencil.Util;
using UnityEngine;

namespace Analytics
{
    public class FirebaseTracking : ITracker
    {
        public ITracker Track(string name, Dictionary<string, object> eventData)
        {
            if (!StencilFirebase.IsReady) return this;
            if (eventData == null)
            {
                FirebaseAnalytics.LogEvent(name);
            }
            else
            {
                FirebaseAnalytics.LogEvent(name, eventData.Select(kv =>
                {
                    try
                    {
                        var value = kv.Value;
                        if (value is double || value is float || value is decimal)
                            return new Parameter(kv.Key, Convert.ToDouble(kv.Value));
                        if (value is long || value is int || value is byte || value is bool)
                            return new Parameter(kv.Key, Convert.ToInt64(kv.Value));
                        if (value is ulong || value is uint)
                            return new Parameter(kv.Key, Convert.ToUInt64(kv.Value));
                        if (value is string || value is char || value is Enum)
                            return new Parameter(kv.Key, $"{kv.Value}");
                        if (value is UInt128 || value is Int128)
                            return new Parameter(kv.Key, kv.Value.ToString());
                        throw new Exception($"Unrecognized tracking type for {kv.Key}: {value.GetType()}");
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        return new Parameter(kv.Key, kv.Value.ToString());
                    }
                }).ToArray());
            }

            #if STENCIL_FIREBASE
            Crashlytics.Log($"{name}: {Json.Serialize(eventData)}");
            #endif
            return this;
        }

        public ITracker SetUserProperty(string name, object value)
        {
            if (!StencilFirebase.IsReady) return this;
            FirebaseAnalytics.SetUserProperty(name, value?.ToString());
            #if STENCIL_FIREBASE
            Crashlytics.Log($"Set Property {name} = {value}");
            Crashlytics.SetCustomKey(name, value?.ToString());
            #endif
            return this;
        }
    }
}
#endif