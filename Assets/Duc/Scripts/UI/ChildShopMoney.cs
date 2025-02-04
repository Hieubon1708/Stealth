using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Duc
{
    public class ChildShopMoney : MonoBehaviour
    {
        [SerializeField] private Image imgIcon;
        [SerializeField] private TextMeshProUGUI txtValue;
        [SerializeField] private TextMeshProUGUI txtPrice;

        private ShopPack data;

        public void Init(ShopPack _data)
        {
            data = _data;

            int value = data.rewards[0].value;
            txtValue.text = value.ToString();

            imgIcon.sprite = data.sprIcon;
            imgIcon.SetNativeSize();

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
                UIEconomy.instance.AddCash(data.rewards[0].value, imgIcon.transform);
                UIManager.instance.ShowTextNotice("Thank you for your purchase!");
                ACEPlay.Bridge.BridgeController.instance.LogEarnCurrency("money", data.rewards[0].value, "shop");
            });
            ACEPlay.Bridge.BridgeController.instance.PurchaseProduct(data.keyIAP, e);
        }
    }
}