using System;
using System.Collections.Generic;
using Analytics;
using Binding;
using Plugins.UI;
using Scripts.RemoteConfig;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Scripts.Tenjin
{
    public class StencilTenjin : Permanent<StencilTenjin>
    {
#if STENCIL_TENJIN
        
        public string apiKey;
        public BaseTenjin tenjin { get; private set; }
        
        [RemoteField("tenjin_base")] 
        public bool baseEnabled = true;

        [RemoteField("tenjin_iap")]
        public bool iapEnabled = true;

        public void ProcessPurchase(Product product) {
            var price = product.metadata.localizedPriceString;
            var currencyCode = product.metadata.isoCurrencyCode;

            var wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(product.receipt);
            if (null == wrapper)
            {
                return;
            }

            var payload   = (string)wrapper["Payload"]; // For Apple this will be the base64 encoded ASN.1 receipt
            var productId = product.definition.id;

            double lPrice = 0;
            double.TryParse(price, out lPrice);
            
            Debug.Log($"Tenjin Full Receipt: {payload}");
#if UNITY_EDITOR
            Debug.LogWarning("Logged transaction for Tenjin. Exiting because Editor");
#elif UNITY_ANDROID
            Tracking.Instance.Track("tenjin_iap_receive");
            var gpDetails = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
            var gpJson    = (string)gpDetails["json"];
            var gpSig     = (string)gpDetails["signature"];
            CompletedAndroidPurchase(productId, currencyCode, 1, lPrice, gpJson, gpSig);
            
            Debug.Log($"Tenjin: details={gpDetails}, json={gpJson}, sig={gpSig}");
#elif UNITY_IOS
            Tracking.Instance.Track("tenjin_iap_receive");
            var transactionId = product.transactionID;
            CompletedIosPurchase(productId, currencyCode, 1, lPrice , transactionId, payload);
#endif
        }

        private void CompletedAndroidPurchase(string ProductId, string CurrencyCode, int Quantity, double UnitPrice, string Receipt, string Signature)
        {
            Transaction(ProductId, CurrencyCode, Quantity, UnitPrice, null, Receipt, Signature);
        }

        private void CompletedIosPurchase(string ProductId, string CurrencyCode, int Quantity, double UnitPrice, string TransactionId, string Receipt)
        {
            Transaction(ProductId, CurrencyCode, Quantity, UnitPrice, TransactionId, Receipt, null);
        }

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

        private void Transaction(string productId, string currencyCode, int quantity, double unitPrice, string transactionId,
            string receipt, string signature)
        {
            Tracking.Instance.Track("tenjin_iap_attempt");
            if (!baseEnabled || !iapEnabled) return;
            Tracking.Instance.Track("tenjin_iap_send");
            tenjin?.Transaction(productId,currencyCode,quantity,unitPrice,transactionId,receipt,signature);
        }
#endif
    }
}
