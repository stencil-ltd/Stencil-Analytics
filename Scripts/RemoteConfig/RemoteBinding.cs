using System.Linq;
using System.Reflection;
using Binding;
using Dev;
using UnityEngine;
using Util;
using Developers = Dev.Developers;

namespace Scripts.RemoteConfig
{
    public static class RemoteBinding
    {
        #if STENCIL_FIREBASE
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
                var key = myAttr.GetKey(obj);
                var config = StencilRemote.GetValue(key);
                if (config.HasValue())
                {
                    var value = config.GetValue(field.FieldType);
                    Debug.Log($"RemoteConfig: {obj} is setting {key} to {value}");
                    field.SetValue(obj, value);
                }
            };
            
            // Properties
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(prop => prop.IsDefined(attr, true));
            foreach (var prop in props)
            {
                var myAttr = prop.GetCustomAttribute<RemoteField>();
                var key = myAttr.GetKey(obj);
                var config = StencilRemote.GetValue(key);
                if (config.HasValue())
                {
                    var value = config.GetValue(prop.PropertyType);
                    Debug.Log($"RemoteConfig: {obj} is setting ${key} to {value}");
                    prop.SetValue(obj, value);
                }
            }
        }
        #else
        public static void BindRemoteConfig(this object obj)
        {
            Debug.LogError("Firebase is not linked. Not binding to remote config.");
        }
        #endif
    }
}