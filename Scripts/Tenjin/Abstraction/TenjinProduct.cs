using System;
using System.Collections.Generic;
using Analytics;
using Scripts.RemoteConfig;
using Scripts.Tenjin.Subscriptions;
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
            return new TenjinProduct(StencilTenjin.Instance, product);
            #elif UNITY_IOS
            return new TenjinProductIos(StencilTenjin.Instance, product);
            #elif UNITY_ANDROID
            return new TenjinProductAndroid(StencilTenjin.Instance, product);
            #else
            throw new Exception("Can't create TenjinProduct. Unknown Platform.");
            #endif
        }

        public readonly StencilTenjin tenjin;
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
        
        // Subscription Only.
        protected SubscriptionState subscription;

        protected TenjinProduct(StencilTenjin tenjin, Product product)
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
            if (product.definition.type == ProductType.Subscription)
                subscription = new SubscriptionState(product);
        }

        public void CheckSubscription()
        {
            try
            {
                if (!IsEnabled()) return;
                if (subscription == null) return;
                Refresh();

                var info = subscription.info;
                var now = DateTime.UtcNow;
                if (info.isSubscribed() != Result.True || info.isExpired() == Result.True)
                {
                    Debug.LogWarning("Not subscribed.");
                    return;
                }
                subscription.FirstPurchaseDate = subscription.FirstPurchaseDate ?? now;
                
                if (info.isFreeTrial() == Result.True)
                {
                    Debug.Log("Free Trial");
                    return;
                }
                subscription.FirstChargeDate = subscription.FirstChargeDate ?? now;

                var last = subscription.LastCharge;
                if (last == null)
                {
                    OnTrackPurchase();
                    subscription.LastCharge = now.Date;
                    return;
                }

                var next = last.Value.AddDays(subscription.repeatDays);
                while (next < now)
                {
                    OnTrackPurchase();
                    subscription.LastCharge = next;
                    next = last.Value.AddDays(subscription.repeatDays);   
                }
            }
            catch (Exception e)
            {
                Tracking.LogException(e);
            }
        }

        public void TrackPurchase()
        {
            try
            {
                if (!IsEnabled()) return;
                if (product.definition.type == ProductType.Subscription)
                {
                    // submethod will call refresh.
                    CheckSubscription();
                }
                else
                {
                    Refresh();
                    OnTrackPurchase();
                }
            }
            catch (Exception e)
            {
                Tracking.LogException(e);
            }
        }

        protected bool IsEnabled()
        {
            var iapEnabled = StencilTenjin.Instance.iapEnabled;
            var baseEnabled = StencilTenjin.Instance.baseEnabled;
            return !StencilRemote.IsProd() || iapEnabled && baseEnabled;
        }

        protected virtual void OnTrackPurchase()
        {
            Debug.Log($"Process Receipt: {productId} {currencyCode} {price} {receipt} {signature}");
            #if STENCIL_TENJIN && !UNITY_EDITOR
            tenjin.tenjin.Transaction(productId, currencyCode, 1, price, transactionId, receipt, signature);
            #endif
        }
        
        protected void CheckNotNull(object field, string name)
        {
            if (field == null) throw new NullReferenceException($"Null {name}");
        }
    }
}