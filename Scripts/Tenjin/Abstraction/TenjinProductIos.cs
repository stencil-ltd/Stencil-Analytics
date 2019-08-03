#if STENCIL_TENJIN && UNITY_IOS
using UniRx.Async;
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
            await new PurchaseReporter(product, CustomReportingClient)
                .ReportIos(receipt, transactionId);
        }
    }
}
#endif