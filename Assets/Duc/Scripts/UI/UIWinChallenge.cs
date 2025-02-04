using DG.Tweening;
using Duc.PoppyTangle;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Duc
{
    public class UIWinChallenge : UI
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Transform iconMoney;
        [SerializeField] private TextMeshProUGUI txtAmountMoney;

        private int moneyClaim;

        public override void Show()
        {
            base.Show();

            moneyClaim = Mathf.RoundToInt((GameplayController.instance.pins.Count / 2f) * 2.5f);
            txtAmountMoney.text = moneyClaim.ToString();

            AudioController.instance.PlaySoundWin();

            DOVirtual.DelayedCall(1f, () =>
            {
                ACEPlay.Native.NativeAds.instance.DisplayNativeAds(true);
                ACEPlay.Bridge.BridgeController.instance.ShowBannerCollapsible();
            });
        }

        public override void Hide()
        {
            base.Hide();

            ACEPlay.Native.NativeAds.instance.DisplayNativeAds(false);
            ACEPlay.Bridge.BridgeController.instance.HideBannerCollapsible();
        }

        public void OnClickButtonContinue()
        {
            UnityEvent e = new UnityEvent();
            e.AddListener(() =>
            {
                canvasGroup.interactable = false;

                UIEconomy.instance.AddCash(moneyClaim, iconMoney);

                DOVirtual.DelayedCall(1.5f, () =>
                {
                    int chapter = Mathf.Min(Manager.instance.Chapter, Manager.instance.levelDataSO.chapters.Count);
                    int stage = Manager.instance.Stage;

                    Chapter currentChapter = Manager.instance.levelDataSO.chapters[chapter - 1];

                    if (currentChapter.stages[stage - 1] == StageType.Tangle)
                    {
                        FadeManager.instance.Fade(() =>
                        {
                            GameplayController.instance.QuitGame();
                            UIManager.instance.UIInGame.Hide();
                            Hide();
                            UIManager.instance.ShowUIHome();
                            GameplayController.instance.SetUp();
                        });
                    }
                    else
                    {
                        FadeManager.instance.LoadScene(2, () =>
                        {
                            GameplayController.instance.QuitGame();
                            UIManager.instance.UIInGame.Hide();
                            Hide();
                            //UIManager.instance.ShowUIHome();
                            AudioController.instance.StopMenuMusic();
                        });
                    }
                });

                ACEPlay.Bridge.BridgeController.instance.LogEarnCurrency("money", moneyClaim, "challenge");
            });
            ACEPlay.Bridge.BridgeController.instance.ShowInterstitial("challenge_win_continue", e);
        }
    }
}