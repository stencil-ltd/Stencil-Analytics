using Scripts.Tenjin.Abstraction;
using UnityEngine.Purchasing;

namespace Scripts.Tenjin
{
    public partial class StencilTenjin
    {
        public void OnProcessPurchase(Product product)
        {
            TenjinProduct.Get(product).TrackPurchase();
        }
    }
}