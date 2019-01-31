using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Binding;
using Dev;
using Newtonsoft.Json;
using UnityEngine;

#if !EXCLUDE_FIREBASE
using Firebase.RemoteConfig;
#endif

namespace Util
{
    public static class RemoteConfigExtensions
    {  
#if !EXCLUDE_FIREBASE
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
            if (type.TypeEquals<int[]>())
                return value.IntArrayValue();
            if (type.TypeEquals<long[]>())
                return value.LongArrayValue();
            if (type.TypeEquals<float[]>())
                return value.FloatArrayValue();
            if (type.TypeEquals<double[]>())
                return value.DoubleArrayValue();
            
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
                 
        public static IEnumerable<byte> ByteArrayValue(this ConfigValue value, IEnumerable<byte> defaultValue)
        {
            return value.HasValue() ? value.ByteArrayValue : defaultValue;
        }
                 
        public static int IntValue(this ConfigValue value, int defaultValue)
        {
            return value.HasValue() ? (int) value.LongValue : defaultValue;
        }

        public static int[] IntArrayValue(this ConfigValue value, int[] defaultValue = null)
        {
            return value.HasValue() ? value.PrimitiveArrayValue<int>() : defaultValue;
        }

        public static long[] LongArrayValue(this ConfigValue value, long[] defaultValue = null)
        {
            return value.HasValue() ? value.PrimitiveArrayValue<long>() : defaultValue;
        }

        public static float[] FloatArrayValue(this ConfigValue value, float[] defaultValue = null)
        {
            return value.HasValue() ? value.PrimitiveArrayValue<float>() : defaultValue;
        }

        public static double[] DoubleArrayValue(this ConfigValue value, double[] defaultValue = null)
        {
            return value.HasValue() ? value.PrimitiveArrayValue<double>() : defaultValue;
        }

        public static bool TypeEquals<T>(this Type t)
        {
            return t.IsEquivalentTo(typeof(T));
        }
        
        private static T[] PrimitiveArrayValue<T>(this ConfigValue value) where T : struct
        {
            return JsonConvert.DeserializeObject<T[]>(value.StringValue);
        }

#endif
    }
}