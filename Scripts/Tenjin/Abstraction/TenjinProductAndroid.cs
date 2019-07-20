#if STENCIL_TENJIN && UNITY_ANDROID

using System.Collections.Generic;
using UnityEngine.Purchasing;

namespace Scripts.Tenjin.Abstraction
{
    public class TenjinProductAndroid : TenjinProduct
    {
        public TenjinProductAndroid(StencilTenjin tenjin, Product product) : base(tenjin, product)
        {}

        protected override void Refresh()
        {
            base.Refresh();
            var gpDetails = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
            CheckNotNull(gpDetails, "gpDetails");
            receipt = (string)gpDetails["json"];
            CheckNotNull(receipt, "gpJson");
            signature = (string)gpDetails["signature"];
            CheckNotNull(signature, "gpSig");
        }
    }
}

#endif