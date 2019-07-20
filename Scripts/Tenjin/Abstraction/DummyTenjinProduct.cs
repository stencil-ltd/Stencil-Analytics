using UnityEngine.Purchasing;

namespace Scripts.Tenjin.Abstraction
{
    public class DummyTenjinProduct : ITenjinProduct
    {
        public Product product { get; }

        public DummyTenjinProduct(Product product)
        {
            this.product = product;
        }

        public void TrackPurchase()
        {
            
        }
    }
}