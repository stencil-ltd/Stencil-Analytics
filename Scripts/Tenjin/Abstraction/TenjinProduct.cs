using System;
using System.Collections.Generic;
using Analytics;
using Scripts.RemoteConfig;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Scripts.Tenjin.Abstraction
{
    public class TenjinProduct : ITenjinProduct
    {
        public static ITenjinProduct Get(Product product)
        {
            #if !STENCIL_TENJIN
            return new DummyTenjinProduct(product);
            #elif UNITY_EDITOR
            return new TenjinProduct(StencilTenjin.Instance.tenjin, product);
            #elif UNITY_IOS
            return new TenjinProductIos(StencilTenjin.Instance.tenjin, product);
            #elif UNITY_ANDROID
            return new TenjinProductAndroid(StencilTenjin.Instance.tenjin, product);
            #else
            throw new Exception("Can't create TenjinProduct. Unknown Platform.");
            #endif
        }

        public readonly BaseTenjin tenjin;
        public Product product { get; }

        protected Dictionary<string, object> wrapper;
        protected double price;
        protected string currencyCode;
        protected string payload;
        protected string productId;

        // Platform-dependent values.
        protected string transactionId;
        protected string receipt;
        protected string signature;

        protected TenjinProduct(BaseTenjin tenjin, Product product)
        {
            this.product = product;
            this.tenjin = tenjin;
        }

        protected virtual void Refresh()
        {
            wrapper = (Dictionary<string, object>) MiniJson.JsonDecode(product.receipt);
            price = decimal.ToDouble(product.metadata.localizedPrice);
            currencyCode = product.metadata.isoCurrencyCode;
            CheckNotNull(currencyCode, "Currency Code");
            payload = (string) wrapper["Payload"];
            productId = product.definition.id;
            CheckNotNull(productId, "Product ID");
        }

        public void TrackPurchase()
        {
            try
            {
                var iapEnabled = StencilTenjin.Instance.iapEnabled;
                var baseEnabled = StencilTenjin.Instance.baseEnabled;
                if (StencilRemote.IsProd() && (!iapEnabled || !baseEnabled)) return;
                Refresh();
                OnTrackPurchase();
            }
            catch (Exception e)
            {
                Tracking.LogException(e);
            }
        }

        protected virtual void OnTrackPurchase()
        {
            Debug.Log($"Process Receipt: {productId} {currencyCode} {price} {receipt} {signature}");
            #if !UNITY_EDITOR
            tenjin.Transaction(productId, currencyCode, 1, price, transactionId, receipt, signature);
            #endif
        }
        
        protected void CheckNotNull(object field, string name)
        {
            if (field == null) throw new NullReferenceException($"Null {name}");
        }
    }
}