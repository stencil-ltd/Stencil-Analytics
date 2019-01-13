using System.Collections.Generic;
using Binding;
using UnityEngine;
using UnityEngine.Purchasing; 

namespace Scripts.Tenjin
{
    [RequireComponent(typeof(IAPListener))]
    public class TenjinIapHelper : MonoBehaviour
    {
#if STENCIL_TENJIN
        [Bind]
        public IAPListener Listener { get; private set; }
        
        private void Awake()
        {
            this.Bind();
            Listener.onPurchaseComplete.AddListener(ProcessPurchase);
        }

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
            
#if UNITY_EDITOR
            Debug.LogWarning("Logged transaction for Tenjin. Exiting because Editor");
#elif UNITY_ANDROID
            var gpDetails = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
            var gpJson    = (string)gpDetails["json"];
            var gpSig     = (string)gpDetails["signature"];
            CompletedAndroidPurchase(productId, currencyCode, 1, lPrice, gpJson, gpSig);
#elif UNITY_IOS
            var transactionId = product.transactionID;
            CompletedIosPurchase(productId, currencyCode, 1, lPrice , transactionId, payload);
#endif
        }

        private void CompletedAndroidPurchase(string ProductId, string CurrencyCode, int Quantity, double UnitPrice, string Receipt, string Signature)
        {
            StencilTenjin.Instance.Transaction(ProductId, CurrencyCode, Quantity, UnitPrice, null, Receipt, Signature);
        }

        private void CompletedIosPurchase(string ProductId, string CurrencyCode, int Quantity, double UnitPrice, string TransactionId, string Receipt)
        {
            StencilTenjin.Instance.Transaction(ProductId, CurrencyCode, Quantity, UnitPrice, TransactionId, Receipt, null);
        }
#endif
    }
}
