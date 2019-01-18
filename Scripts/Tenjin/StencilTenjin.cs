using System;
using Binding;
using Plugins.UI;
using Scripts.RemoteConfig;
using UnityEngine;

namespace Scripts.Tenjin
{
    public partial class StencilTenjin : Permanent<StencilTenjin>
    {
#if STENCIL_TENJIN
        
        public string apiKey;
        public BaseTenjin tenjin { get; private set; }
        
        [RemoteField("tenjin_base")] 
        public bool baseEnabled = true;

        [RemoteField("tenjin_iap")]
        public bool iapEnabled = true;

        protected override void Awake()
        {
            base.Awake();
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
