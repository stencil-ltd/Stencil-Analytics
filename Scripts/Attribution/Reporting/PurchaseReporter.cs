#if UNITY_PURCHASING
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using Scripts.Tenjin.Stores.Google;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Purchasing;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

#if STENCIL_JSON_NET
using Newtonsoft.Json;
#endif

namespace Scripts.Tenjin
{
    public class PurchaseReporter
    {
        private static readonly string googleEndpoint = "googleRegisterPurchase";
        private static readonly string appleEndpoint = "appleRegisterPurchase";
        
        public readonly Product product;
        public readonly HttpClient client;

        private string _adId = null;

        public PurchaseReporter(Product product, HttpClient client)
        {
            this.product = product;
            this.client = client;
            Application.RequestAdvertisingIdentifierAsync((id, enabled, msg) => _adId = id);
        }

        private async UniTask<RegisterPayload> CreatePayload(string receipt)
        {
            await UniTask.WaitWhile(() => _adId == null);
            var locale = CultureInfo.CurrentCulture.DisplayName;
            return new RegisterPayload
            {
                product_id = product.definition.id,
                price = (double) product.metadata.localizedPrice,
                receipt = receipt,
                advertising_id = _adId,
                bundle_id = Application.identifier,
                country = locale.Split('-').Last(),
                currency = product.metadata.isoCurrencyCode,
                // TODO os_version
                // TODO os_version_release,
                app_version = Application.version,
                // TODO build_id,
                locale = locale,
                device_model = SystemInfo.deviceModel
            };
        }

        private StringContent CreateContent(RegisterPayload payload)
        {
            var json = "";
            #if STENCIL_JSON_NET
            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            json = JsonConvert.SerializeObject(payload, jsonSettings);
            #else
                json = JsonUtility.ToJson(payload);
            #endif
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        public async UniTask<HttpResponseMessage> ReportAndroid(string receipt, string signature)
        {
            if (client == null) return null;
            var payload = await CreatePayload(receipt);
            payload.signature = signature;
            return await client.PostAsync(googleEndpoint, CreateContent(payload));
        }

        public async UniTask<HttpResponseMessage> ReportIos(string receipt, string transactionId)
        {
            if (client == null) return null;
            var payload = await CreatePayload(receipt);
            payload.transaction_id = transactionId;
            #if UNITY_IOS
            payload.developer_device_id = Device.vendorIdentifier;
            #endif
            return await client.PostAsync(appleEndpoint, CreateContent(payload));
        }
    }
}

#endif