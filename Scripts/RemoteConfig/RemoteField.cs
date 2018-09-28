using System;

namespace Binding
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RemoteField : Attribute
    {
        public static bool SkipForDevelopers = false;
        
        public readonly string Key;
        public RemoteField(string key)
        {
            Key = key;
        }
    }
}