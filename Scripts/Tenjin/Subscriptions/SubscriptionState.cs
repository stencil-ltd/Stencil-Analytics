using System;
using Scripts.Prefs;
using UnityEngine.Purchasing;

namespace Scripts.Tenjin.Subscriptions
{
    public class SubscriptionState
    {
        public readonly string id;
        public readonly Product product;
        public readonly SubscriptionInfo info;

        public int freeDays = 3;
        public int repeatDays = 7;

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

        public SubscriptionState(Product product, int freeDays = 3)
        {
            this.product = product;
            id = product.definition.id;
            this.freeDays = freeDays;
            info = new SubscriptionManager(product, null).getSubscriptionInfo();
        }
    }
}