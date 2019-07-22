using System;
using System.Collections.Generic;
using Analytics;
using JetBrains.Annotations;
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

        protected double price;
        protected string currencyCode;
        protected string productId;

        [CanBeNull] protected Dictionary<string, object> wrapper;
        [CanBeNull] protected string payload;

        // Platform-dependent values.
        [CanBeNull] protected string transactionId;
        [CanBeNull] protected string receipt;
        [CanBeNull] protected string signature;
        
        // Subscription Only.
        [CanBeNull] protected SubscriptionState subscription;

        protected TenjinProduct(StencilTenjin tenjin, Product product)
        {
            this.product = product;
            this.tenjin = tenjin;
        }

        protected virtual void Refresh()
        {
            Debug.Log($"TenjinProduct: Refresh {productId}");
            productId = product.definition.id;
            CheckNotNull(productId, "Product ID");
            price = decimal.ToDouble(product.metadata.localizedPrice);
            currencyCode = product.metadata.isoCurrencyCode;
            CheckNotNull(currencyCode, "Currency Code");
            if (product.receipt == null)
            {
                Debug.Log($"TenjinProduct: No Receipt {productId}");
                wrapper = null;
                payload = null;
                subscription = null;
                return;
            }
            wrapper = (Dictionary<string, object>) MiniJson.JsonDecode(product.receipt);
            payload = (string) wrapper["Payload"];
            if (product.definition.type == ProductType.Subscription)
                subscription = new SubscriptionState(product);
        }

        public void CheckSubscription()
        {
            try
            {
                if (!IsEnabled()) return;
                
                Debug.Log($"TenjinProduct: Check Subscription {productId}");
                Refresh();
                
                if (subscription == null)
                    return;

                var info = subscription.info;
                var now = DateTime.UtcNow;
                if (info.isSubscribed() != Result.True || info.isExpired() == Result.True)
                {
                    Debug.LogWarning($"TenjinProduct: Not subscribed {productId}");
                    subscription.Clear();
                    return;
                }
                subscription.FirstPurchaseDate = subscription.FirstPurchaseDate ?? now;
                
                if (info.isFreeTrial() == Result.True)
                {
                    Debug.Log($"TenjinProduct: Free Trial {productId}");
                    return;
                }
                subscription.FirstChargeDate = subscription.FirstChargeDate ?? now;

                var last = subscription.LastCharge;
                if (last == null)
                {
                    Debug.Log($"TenjinProduct: First Charge: {productId}");
                    OnTrackPurchase();
                    subscription.LastCharge = now.Date;
                    return;
                }

                var next = last.Value + subscription.repeatInterval;
                var count = 0;
                while (next < now)
                {
                    Debug.Log($"TenjinProduct: One valid charge {productId}");
                    count++;
                    next += subscription.repeatInterval;
                }
                if (count > 0)
                {
                    Debug.Log($"TenjinProduct: Charge x{count} {productId}");
                    OnTrackPurchase(count);
                    subscription.LastCharge = next;
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
                Debug.Log($"TenjinProduct: TrackPurchase {productId}");
                if (product.definition.type == ProductType.Subscription)
                {
                    // submethod will call refresh.
                    Debug.Log($"TenjinProduct: TrackPurchase -> CheckSubscription {productId}");
                    CheckSubscription();
                }
                else
                {
                    Debug.Log($"TenjinProduct: Valid Consumable Purchase {productId}");
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
            var prod = StencilRemote.IsProd();
            var iapEnabled = StencilTenjin.Instance.iapEnabled;
            var baseEnabled = StencilTenjin.Instance.baseEnabled;
            var retval = !prod || iapEnabled && baseEnabled;
            Debug.Log($"TenjinProduct: Enabled = {retval} (!{prod} and {baseEnabled} and {iapEnabled}): {productId}");
            return retval;
        }

        protected virtual void OnTrackPurchase(int count = 1)
        {
            Debug.Log($"TenjinProduct: Process Receipt: {productId} {currencyCode} {price} {receipt} {signature}");
            if (!StencilRemote.IsProd()) return;
            #if !STENCIL_TENJIN || UNITY_EDITOR
            return;
            #endif
            tenjin.tenjin.Transaction(productId, currencyCode, count, price, transactionId, receipt, signature);
        }
        
        protected void CheckNotNull(object field, string name)
        {
            if (field == null) throw new NullReferenceException($"Null {name}");
        }
    }
}