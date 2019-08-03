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
            var os = SystemInfo.operatingSystem;
            var osVersion = os.Split(' ').Last();
            if (Application.platform == RuntimePlatform.Android) 
                osVersion = osVersion.Replace("API-", "");
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
                os_version = osVersion,
                // TODO os_version_release,
                app_version = Application.version,
                // TODO build_id,
                locale = locale,
                device_model = SystemInfo.deviceModel
            };
        }

        public async UniTask ReportAndroid(string receipt, string signature)
        {
            if (client == null) return;
            var payload = await CreatePayload(receipt);
            payload.signature = signature;
            var content = new StringContent(JsonUtility.ToJson(payload), Encoding.UTF8, "application/json");
            await client.PostAsync(googleEndpoint, content);
        }

        public async UniTask ReportIos(string receipt, string transactionId)
        {
            if (client == null) return;
            var payload = await CreatePayload(receipt);
            payload.transaction_id = transactionId;
            #if UNITY_IOS
            payload.developer_device_id = Device.vendorIdentifier;
            #endif
            var content = new StringContent(JsonUtility.ToJson(payload), Encoding.UTF8, "application/json");
            await client.PostAsync(appleEndpoint, content);
        }
    }
}