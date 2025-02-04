using TigerForge;
using TMPro;
using UnityEngine;

namespace Duc
{
    public class ChildShopRemoveAds : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtPrice;
        [SerializeField] private ButtonScale btnBuy;

        private ShopPack data;
        
        public void Init(ShopPack _data)
        {
            data = _data;

            btnBuy.interactable = true;

            if (!string.IsNullOrEmpty(data.keyIAP))
            {
                string localizedPrice = Manager.instance.GetProductPrice(data.keyIAP);
                if (!string.IsNullOrEmpty(localizedPrice))
                {
                    txtPrice.text = localizedPrice;
                }
                else
                {
                    txtPrice.text = string.Format("$ {0}", data.price);
                }
            }
            else
            {
                txtPrice.text = string.Empty;
            }
        }

        public void OnClick()
        {
            UnityStringEvent e = new UnityStringEvent();
            e.AddListener(result =>
            {
                btnBuy.interactable = false;
                ACEPlay.Bridge.BridgeController.instance.CanShowAds = false;
                EventManager.EmitEvent(EventVariables.RemoveAds);
                UIManager.instance.ShowTextNotice("Thank you for your purchase!");
            });
            ACEPlay.Bridge.BridgeController.instance.PurchaseProduct(data.keyIAP, e);
        }
    }
}