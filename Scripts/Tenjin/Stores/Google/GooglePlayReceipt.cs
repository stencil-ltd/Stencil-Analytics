using System;

namespace Scripts.Tenjin.Stores.Google
{
    [Serializable]
    public class GooglePlayReceipt
    {
        public string orderId;
        public string packageName;
        public string productId;
        public long purchaseTime;
        public int purchaseState;
        public string developerPayload;
        public string purchaseToken;
        public bool autoRenewing;
    }
}