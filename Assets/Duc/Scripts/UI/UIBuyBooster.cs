using Duc.PoppyTangle;
using System.Collections.Generic;
using TigerForge;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duc
{
    public class UIBuyBooster : UI
    {
        [SerializeField] private TextMeshProUGUI txtBoosterName;
        [SerializeField] private TextMeshProUGUI txtBoosterDescription;
        [SerializeField] private List<Image> imgBoosters;
        [SerializeField] private TextMeshProUGUI txtPrice;
        [SerializeField] private Sprite[] sprBoosters;

        private Item boosterType;

        private int priceAxe = 1800;
        private int priceHammer = 1800;
        private int priceFreeze = 1500;

        public override void Start()
        {
            base.Start();
        }

        public override void Show()
        {
            base.Show();

            boosterType = (Item)EventManager.GetData(EventVariables.BuyBooster);

            switch (boosterType)
            {
                case Item.Axe:
                    txtBoosterName.text = "Axe";
                    txtBoosterDescription.text = "Tap any chain to cut it with axe!";
                    imgBoosters.ForEach(img => img.sprite = sprBoosters[0]);
                    txtPrice.text = priceAxe.ToString();
                    break;
                case Item.Hammer:
                    txtBoosterName.text = "Hammer";
                    txtBoosterDescription.text = "Tap any locked pin to break it with hammer!";
                    imgBoosters.ForEach(img => img.sprite = sprBoosters[1]);
                    txtPrice.text = priceHammer.ToString();
                    break;
                case Item.FreezeTime:
                    txtBoosterName.text = "Freeze Time";
                    txtBoosterDescription.text = "Freeze the time for a short period!";
                    imgBoosters.ForEach(img => img.sprite = sprBoosters[2]);
                    txtPrice.text = priceFreeze.ToString();
                    break;
            }

            GameplayController.instance.gameplayStatus = GameplayStatus.Pause;

            ACEPlay.Native.NativeAds.instance.DisplayNativeAds(true);
            ACEPlay.Bridge.BridgeController.instance.ShowBannerCollapsible();
        }

        public override void Hide()
        {
            UIManager.instance.UIInGame.DisplayButtonAxe();
            UIManager.instance.UIInGame.DisplayButtonHammer();
            UIManager.instance.UIInGame.DisplayButtonFreeze();

            GameplayController.instance.gameplayStatus = GameplayStatus.Playing;

            ACEPlay.Native.NativeAds.instance.DisplayNativeAds(false);
            ACEPlay.Bridge.BridgeController.instance.HideBannerCollapsible();

            base.Hide();
        }

        public void OnClickButtonBuy()
        {
            int price = 0;
            switch (boosterType)
            {
                case Item.Axe:
                    price = priceAxe;
                    break;
                case Item.Hammer:
                    price = priceHammer;
                    break;
                case Item.FreezeTime:
                    price = priceFreeze;
                    break;
            }

            int cash = Manager.instance.Money;
            if (cash >= price)
            {
                Manager.instance.Money = cash - price;
                UIEconomy.instance.DisplayTotalCash(true, cash);

                switch (boosterType)
                {
                    case Item.Axe:
                        Manager.instance.Axe += 3;
                        break;
                    case Item.Hammer:
                        Manager.instance.Hammer += 3;
                        break;
                    case Item.FreezeTime:
                        Manager.instance.FreezeTime += 3;
                        break;
                }

                Manager.instance.TotalSpend += price;
                ACEPlay.Bridge.BridgeController.instance.SetPropertyCoinSpend(Manager.instance.TotalSpend.ToString());
                ACEPlay.Bridge.BridgeController.instance.LogSpendCurrency("money", price, "booster");

                Hide();
            }
            else
            {
                UIManager.instance.ShowTextNotice("Not enough money!");
                UIManager.instance.ShowUIShop(false);

                ACEPlay.Native.NativeAds.instance.DisplayNativeAds(false);
                ACEPlay.Bridge.BridgeController.instance.HideBannerCollapsible();
            }

        }

        public void OnClickButtonWatchAds()
        {
            if (ACEPlay.Bridge.BridgeController.instance.IsRewardReady())
            {
                UnityEvent e = new UnityEvent();
                e.AddListener(() =>
                {
                    switch (boosterType)
                    {
                        case Item.Axe:
                            Manager.instance.Axe += 1;
                            break;
                        case Item.Hammer:
                            Manager.instance.Hammer += 1;
                            break;
                        case Item.FreezeTime:
                            Manager.instance.FreezeTime += 1;
                            break;
                    }

                    Hide();
                });
                ACEPlay.Bridge.BridgeController.instance.ShowRewarded($"buybooster_{boosterType}", e);
            }
            else
            {
                UIManager.instance.ShowTextNotice("Ad is not ready!");
            }
        }

        public void OnClickButtonClose()
        {
            Hide();
        }
    }
}