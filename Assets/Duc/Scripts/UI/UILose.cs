using Duc.PoppyTangle;
using TigerForge;
using UnityEngine.Events;

namespace Duc
{
    public class UILose : UI
    {
        public override void Start()
        {
            base.Start();
        }

        public override void Show()
        {
            base.Show();

            ACEPlay.Native.NativeAds.instance.DisplayNativeAds(true);
            ACEPlay.Bridge.BridgeController.instance.ShowBannerCollapsible();
        }

        public override void Hide()
        {
            ACEPlay.Native.NativeAds.instance.DisplayNativeAds(false);
            ACEPlay.Bridge.BridgeController.instance.HideBannerCollapsible();

            base.Hide();
        }

        public void OnClickButtonWatchAds()
        {
            UnityEvent e = new UnityEvent();
            e.AddListener(() =>
            {
                EventManager.EmitEventData(EventVariables.CountDown, 45f);
                GameplayController.instance.gameplayStatus = GameplayStatus.Playing;
                Hide();
            });
            ACEPlay.Bridge.BridgeController.instance.ShowRewarded("tangle_revive", e);
        }

        public void OnClickButtonBuy()
        {
            int cash = Manager.instance.Money;
            if (cash >= 350)
            {
                Manager.instance.Money = cash - 350;
                UIEconomy.instance.DisplayTotalCash(true, cash);
                Manager.instance.TotalSpend += 350;

                EventManager.EmitEventData(EventVariables.CountDown, 60f);
                GameplayController.instance.gameplayStatus = GameplayStatus.Playing;

                ACEPlay.Bridge.BridgeController.instance.SetPropertyCoinSpend(Manager.instance.TotalSpend.ToString());

                if (GameplayController.instance.gameMode == GameMode.Tangle)
                {
                    ACEPlay.Bridge.BridgeController.instance.LogSpendCurrency("money", 350, "tangle_revive");
                }
                else
                {
                    ACEPlay.Bridge.BridgeController.instance.LogSpendCurrency("money", 350, "challenge_revive");
                }
                Hide();
            }
            else
            {
                UIManager.instance.ShowTextNotice("Not enough cash!");
                UIManager.instance.ShowUIShop(false);
                ACEPlay.Native.NativeAds.instance.DisplayNativeAds(false);
                ACEPlay.Bridge.BridgeController.instance.HideBannerCollapsible();
            }
        }

        public void OnClickButtonReplay()
        {
            string placement;
            if (GameplayController.instance.gameMode == GameMode.Tangle)
            {
                placement = "tangle_lose";
            }
            else
            {
                placement = "challenge_lose";
            }

            UnityEvent e = new UnityEvent();
            e.AddListener(() =>
            {
                FadeManager.instance.Fade(() =>
                {
                    Hide();
                    GameplayController.instance.Init();

                    if (GameplayController.instance.gameMode == GameMode.Tangle)
                    {
                        ACEPlay.Bridge.BridgeController.instance.LogLevelFailWithParameter("tangle", Manager.instance.LevelPoppyTangle);
                    }
                    else
                    {
                        ACEPlay.Bridge.BridgeController.instance.LogLevelFailWithParameter("challenge", MissionManager.instance.LevelTwistedChallenge);
                    }
                });
            });
            ACEPlay.Bridge.BridgeController.instance.ShowInterstitial(placement, e);
        }
    }
}