using System;
using System.Collections.Generic;
using Analytics;
using Scripts.RemoteConfig;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Scripts.Tenjin
{
    public partial class StencilTenjin
    {
        public void OnProcessPurchase(Product product)
        {
            try
            {
                if (StencilRemote.IsProd() && (!iapEnabled || !baseEnabled)) return;
                var price = product.metadata.localizedPrice;
                var lPrice = decimal.ToDouble(price);
                var currencyCode = product.metadata.isoCurrencyCode;

                var wrapper = (Dictionary<string, object>) MiniJson.JsonDecode(product.receipt);
                if (null == wrapper)
                {
                    return;
                }

                var payload = (string) wrapper["Payload"]; // For Apple this will be the base64 encoded ASN.1 receipt
                var productId = product.definition.id;

                CheckNotNull(currencyCode, "Currency Code");
                CheckNotNull(productId, "Product ID");
                CheckNotNull(payload, "Payload");

#if UNITY_EDITOR
                Debug.Log("Tenjin doesn't work in editor");
#elif UNITY_ANDROID
                var gpDetails = (Dictionary<string, object>)MiniJson.JsonEncode(payload);
                CheckNotNull(gpDetails, "gpDetails");
                var gpJson = (string)gpDetails["json"];
                CheckNotNull(gpJson, "gpJson");
                var gpSig = (string)gpDetails["signature"];
                CheckNotNull(gpSig, "gpSig");
                CompletedAndroidPurchase(productId, currencyCode, 1, lPrice, gpJson, gpSig);
#elif UNITY_IOS
                var transactionId = product.transactionID;
                CheckNotNull(transactionId, "transactionId");
                CompletedIosPurchase(productId, currencyCode, 1, lPrice , transactionId, payload);
#endif
            }
            catch (Exception e)
            {
                Tracking.LogException(e);
            }
        }

        private void CheckNotNull(object field, string name)
        {
            if (field == null)
                throw new NullReferenceException($"Null {name}");
        }

        private void CompletedAndroidPurchase(string ProductId, string CurrencyCode, int Quantity, double UnitPrice, string Receipt, string Signature)
        {
            Debug.Log($"Process Receipt: {ProductId} {CurrencyCode} {Quantity} {UnitPrice} {Receipt} {Signature}");
#if STENCIL_TENJIN
            tenjin.Transaction(ProductId, CurrencyCode, Quantity, UnitPrice, null, Receipt, Signature);
#endif
        }

        private void CompletedIosPurchase(string ProductId, string CurrencyCode, int Quantity, double UnitPrice, string TransactionId, string Receipt)
        {
            Debug.Log($"Process Receipt: {ProductId} {CurrencyCode} {Quantity} {UnitPrice} {TransactionId} {Receipt}");
#if STENCIL_TENJIN
            tenjin.Transaction(ProductId, CurrencyCode, Quantity, UnitPrice, TransactionId, Receipt, null);
#endif
        }
    }
}