using System.Collections.Generic;
using Stencil.Util;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Scripts.Tenjin
{
    public partial class StencilTenjin
    {
        public void OnProcessPurchase(Product product)
        {
            if (!iapEnabled || !baseEnabled) return;
            var price = product.metadata.localizedPrice;
            double lPrice = decimal.ToDouble(price);
            var currencyCode = product.metadata.isoCurrencyCode;

            var wrapper = (Dictionary<string, object>) Json.Deserialize(product.receipt);
            if (null == wrapper) {
                return;
            }

            var payload   = (string)wrapper["Payload"]; // For Apple this will be the base64 encoded ASN.1 receipt
            var productId = product.definition.id;
            
#if UNITY_EDITOR
            Debug.Log("Tenjin doesn't work in editor");
#elif UNITY_ANDROID
            var gpDetails = (Dictionary<string, object>)Json.Deserialize(payload);
            var gpJson = (string)gpDetails["json"];
            var gpSig = (string)gpDetails["signature"];
            CompletedAndroidPurchase(productId, currencyCode, 1, lPrice, gpJson, gpSig);
#elif UNITY_IOS
            var transactionId = product.transactionID;
            CompletedIosPurchase(productId, currencyCode, 1, lPrice , transactionId, payload);
#endif
        }

        private void CompletedAndroidPurchase(string ProductId, string CurrencyCode, int Quantity, double UnitPrice, string Receipt, string Signature)
        {
#if STENCIL_TENJIN
            tenjin.Transaction(ProductId, CurrencyCode, Quantity, UnitPrice, null, Receipt, Signature);
#endif
        }

        private void CompletedIosPurchase(string ProductId, string CurrencyCode, int Quantity, double UnitPrice, string TransactionId, string Receipt)
        {
#if STENCIL_TENJIN
            tenjin.Transaction(ProductId, CurrencyCode, Quantity, UnitPrice, TransactionId, Receipt, null);
#endif
        }
    }
}