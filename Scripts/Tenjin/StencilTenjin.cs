using System;
using Binding;
using Plugins.UI;
using Scripts.RemoteConfig;
using UI;
using UnityEngine;

namespace Scripts.Tenjin
{
    public partial class StencilTenjin : PermanentV2<StencilTenjin>
    {
        public string apiKey;
        
        [RemoteField("tenjin_base")] 
        public bool baseEnabled = true;

        [RemoteField("tenjin_iap_v2")]
        public bool iapEnabled = true;
        
#if STENCIL_TENJIN

        public BaseTenjin tenjin { get; private set; }

        protected override void OnAwake()
        {
            base.OnAwake();
            if (!Valid) return;
            this.BindRemoteConfig();
            if (string.IsNullOrEmpty(apiKey))
            {
                Debug.LogException(new NullReferenceException("Tenjin key is null. Cannot report IAP"));
                return;
            }
            tenjin = global::Tenjin.getInstance(apiKey);
            Connect();
        }

        public void Connect()
        {
            if (!baseEnabled) return;
            if (tenjin == null)
                Debug.LogException(new NullReferenceException("Tenjin is null. Cannot report IAP"));
            Debug.Log("Connect to Tenjin");
            tenjin?.Connect();            
        }
        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus) Connect();
        }
#endif
    }
}
