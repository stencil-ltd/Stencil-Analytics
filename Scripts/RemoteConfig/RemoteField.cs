using System;
using Scripts.RemoteConfig;

namespace Binding
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RemoteField : Attribute
    {
        public static bool SkipForDevelopers = false;
        
        public readonly string Key;
        public readonly bool ProcessId;
        
        public RemoteField(string key, bool processId = true)
        {
            Key = key;
            ProcessId = processId;
        }

        public string GetKey(object owner)
        {
            if (!ProcessId) return Key;
            return owner is IRemoteId proc ? proc.ProcessRemoteId(Key) : Key;
        }
    }
}