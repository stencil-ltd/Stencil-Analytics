using System;
using JetBrains.Annotations;

namespace Scripts.Tenjin.Stores.Google
{
    [Serializable]
    public class RegisterPayload
    {
        public string product_id;
        public double price;
        public string receipt;
        public string advertising_id;
        public string bundle_id;
        public string country;
        public string currency;    
        public string os_version;
        public string os_version_release;
        public string app_version;
        public string build_id;
        public string locale;
        public string device_model;
        
        // Android Only
        [CanBeNull] public string signature;
        
        // Apple Only
        [CanBeNull] public string transaction_id;
        [CanBeNull] public string developer_device_id;
    }
}