#if STENCIL_TENJIN && UNITY_IOS
using UnityEngine.Purchasing;

namespace Scripts.Tenjin.Abstraction
{
    public class TenjinProductIos : TenjinProduct
    {

        public TenjinProductIos(BaseTenjin tenjin, Product product) : base(tenjin, product)
        {}

        protected override void Refresh()
        {
            base.Refresh();
            transactionId = product.transactionID;
            receipt = payload;
            signature = null;
            CheckNotNull(transactionId, "transactionId");   
        }
    }
}
#endif