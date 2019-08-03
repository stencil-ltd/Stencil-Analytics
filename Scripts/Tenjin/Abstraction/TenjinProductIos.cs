#if STENCIL_TENJIN && UNITY_IOS
using System;
using Analytics;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Scripts.Tenjin.Abstraction
{
    public class TenjinProductIos : TenjinProduct
    {
        public TenjinProductIos(StencilTenjin tenjin, Product product) : base(tenjin, product)
        {}

        protected override void Refresh()
        {
            base.Refresh();
            transactionId = product.transactionID;
            receipt = payload;
            signature = null; 
        }

        public override async UniTask ReportSubscriptionPurchase()
        {
            try
            {
                await new PurchaseReporter(product, CustomReportingClient)
                    .ReportIos(receipt, transactionId);
                Debug.Log($"Tenjin: Successfully registered purchase with firebase.");
            }
            catch (Exception e)
            {
                Tracking.LogException(e);
            }
        }
    }
}
#endif