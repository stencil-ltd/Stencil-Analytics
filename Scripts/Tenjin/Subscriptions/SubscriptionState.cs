using System;
using Scripts.Prefs;
using Scripts.RemoteConfig;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Scripts.Tenjin.Subscriptions
{
    public class SubscriptionState
    {
        public readonly string id;
        public readonly Product product;
        public readonly SubscriptionInfo info;

        public readonly TimeSpan freeInterval;
        public readonly TimeSpan repeatInterval;

        public readonly bool prod;

        public DateTime? FirstPurchaseDate
        {
            get => StencilPrefs.Default.GetDateTime($"stencil_sub_first_purchase_{id}");
            set => StencilPrefs.Default.SetDateTime($"stencil_sub_first_purchase_{id}", value).Save();
        }  
        
        public DateTime? FirstChargeDate
        {
            get => StencilPrefs.Default.GetDateTime($"stencil_sub_first_charge_{id}");
            set => StencilPrefs.Default.SetDateTime($"stencil_sub_first_charge_{id}", value).Save();
        }
        
        public DateTime? LastCharge
        {
            get => StencilPrefs.Default.GetDateTime($"stencil_sub_last_charge_{id}");
            set => StencilPrefs.Default.SetDateTime($"stencil_sub_last_charge_{id}", value).Save();
        }

        public SubscriptionState(Product product)
        {
            this.product = product;
            prod = StencilRemote.IsProd();
            id = product.definition.id;
            info = new SubscriptionManager(product, null).getSubscriptionInfo();
            freeInterval = prod ? info.getFreeTrialPeriod() : TimeSpan.FromMinutes(3);
            repeatInterval = prod ? info.getSubscriptionPeriod() : TimeSpan.FromMinutes(5);
            Debug.Log($"TenjinProduct: {id} {freeInterval.TotalDays} -> {repeatInterval.TotalDays}");
        }

        public void Clear()
        {
            FirstPurchaseDate = null;
            FirstChargeDate = null;
            LastCharge = null;
        }
    }
}