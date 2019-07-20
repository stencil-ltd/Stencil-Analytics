using UnityEngine.Purchasing;

namespace Scripts.Tenjin.Abstraction
{
    public interface ITenjinProduct
    {
        Product product { get; }
        void TrackPurchase();
    }
}