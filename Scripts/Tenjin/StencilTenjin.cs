using Binding;
using Plugins.UI;
using Scripts.RemoteConfig;
using UnityEngine;

namespace Scripts.Tenjin
{
    [RequireComponent(typeof(TenjinIapHelper))]
    public class StencilTenjin : Permanent<StencilTenjin>
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
            if (string.IsNullOrEmpty(apiKey)) return;
            tenjin = global::Tenjin.getInstance(apiKey);
            Connect();
        }

        public void Connect()
        {
            if (baseEnabled) tenjin?.Connect();
        }
        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus) Connect();
        }

        public void Transaction(string productId, string currencyCode, int quantity, double unitPrice, string transactionId,
            string receipt, string signature)
        {
            if (!baseEnabled || !iapEnabled) return;
            tenjin?.Transaction(productId,currencyCode,quantity,unitPrice,transactionId,receipt,signature);
        }
#endif
    }
}
