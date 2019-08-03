//#if STENCIL_TENJIN && UNITY_ANDROID

using System;
using System.Collections.Generic;
using System.Net.Http;
using Analytics;
using UniRx.Async;
using UnityEngine;
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
            if (payload == null) return;
            var gpDetails = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
            CheckNotNull(gpDetails, "gpDetails");
            receipt = (string)gpDetails["json"];
            CheckNotNull(receipt, "gpJson");
            signature = (string)gpDetails["signature"];
            CheckNotNull(signature, "gpSig");
        }

        public override async UniTask ReportSubscriptionPurchase()
        {
            try
            {
                var response = await new PurchaseReporter(product, CustomReportingClient)
                    .ReportAndroid(receipt, signature);
                if (response?.IsSuccessStatusCode != false)
                {
                    Debug.Log($"Tenjin: Successfully registered purchase with firebase.");                    
                }
                else
                {
                    Debug.LogError($"Tenjin: Could not report to firebase: {response.ReasonPhrase}");
                }
            }
            catch (Exception e)
            {
                Tracking.LogException(e);
            }
        }
    }
}

//#endif