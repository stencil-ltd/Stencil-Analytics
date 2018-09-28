using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Binding;
using Dev;
using Firebase.RemoteConfig;
using UnityEngine;

namespace Util
{
    public static class RemoteConfigExtensions
    {  
    #if !EXCLUDE_FIREBASE
        
        public static void BindRemoteConfig(this object obj)
        {
            if (RemoteField.SkipForDevelopers && Developers.Enabled) return;
            var attr = typeof(RemoteField);
            var type = obj.GetType();
            
            // Fields
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.IsDefined(attr, true));
            foreach (var field in fields)
            {
                var myAttr = field.GetCustomAttribute<RemoteField>();
                var config = FirebaseRemoteConfig.GetValue(myAttr.Key);
                if (config.HasValue())
                {
                    var value = config.GetValue(field.FieldType);
                    Debug.Log($"RemoteConfig: {obj} is setting {myAttr.Key} to {value}");
                    field.SetValue(obj, value);
                }
            };
            
            // Properties
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(prop => prop.IsDefined(attr, true));
            foreach (var prop in props)
            {
                var myAttr = prop.GetCustomAttribute<RemoteField>();
                var config = FirebaseRemoteConfig.GetValue(myAttr.Key);
                if (config.HasValue())
                {
                    var value = config.GetValue(prop.PropertyType);
                    Debug.Log($"RemoteConfig: {obj} is setting ${myAttr.Key} to {value}");
                    prop.SetValue(obj, value);
                }
            }
        }

        public static bool HasValue(this ConfigValue value)
        {
            return value.Source != ValueSource.StaticValue;
        }

        public static object GetValue(this ConfigValue value, Type type)
        {
            if (type.TypeEquals<string>())
                return value.StringValue;
            if (type.TypeEquals<long>())
                return value.LongValue;
            if (type.TypeEquals<int>())
                return Convert.ToInt32(value.LongValue);
            if (type.TypeEquals<double>())
                return value.DoubleValue;
            if (type.TypeEquals<float>())
                return Convert.ToSingle(value.DoubleValue);
            if (type.TypeEquals<bool>())
                return value.BooleanValue;
            
            throw new Exception($"RemoteConfig got confused by {type}");
        }

        public static long LongValue(this ConfigValue value, long defaultValue)
        {
            return value.HasValue() ? value.LongValue : defaultValue;
        }

        public static string StringValue(this ConfigValue value, string defaultValue)
        {
            return value.HasValue() ? value.StringValue : defaultValue;
        }
        
        public static bool BoolValue(this ConfigValue value, bool defaultValue)
        {
            return value.HasValue() ? value.BooleanValue : defaultValue;
        }
        
        public static double DoubleValue(this ConfigValue value, double defaultValue)
        {
            return value.HasValue() ? value.DoubleValue : defaultValue;
        }
        
        public static float FloatValue(this ConfigValue value, float defaultValue)
        {
            return value.HasValue() ? (float) value.DoubleValue : defaultValue;
        }
        
        public static IEnumerable<byte> DoubleValue(this ConfigValue value, IEnumerable<byte> defaultValue)
        {
            return value.HasValue() ? value.ByteArrayValue : defaultValue;
        }
        
        public static int IntValue(this ConfigValue value, int defaultValue)
        {
            return value.HasValue() ? (int) value.LongValue : defaultValue;
        }

        public static bool TypeEquals<T>(this Type t)
        {
            return t.IsEquivalentTo(typeof(T));
        }
    #else
        public static void BindRemoteConfig(this object obj)
        {
            Debug.LogError("Firebase is not linked. Not binding to remote config.");
        }
    #endif
    }
}