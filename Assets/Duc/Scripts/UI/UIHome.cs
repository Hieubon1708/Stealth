using DG.Tweening;
using Duc.PoppyTangle;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;

namespace Duc
{
    public class UIHome : UI
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Transform btnPlay;
        [SerializeField] private RectTransform bottomBar;
        [SerializeField] private GameObject groupButtonTop;
        [SerializeField] private GameObject btnVip;
        [SerializeField] private GameObject btnRemoveAds;
        [SerializeField] private GameObject notiMission;

        private ShopPack removeAdsPack;

        public override void Start()
        {
            base.Start();

            foreach (var shopPack in UIManager.instance.shopDataSO.shopPacks)
            {
                if (shopPack.Name.Contains("Remove Ads"))
                {
                    removeAdsPack = shopPack;
                    break;
                }
            }

            EventManager.StartListening(EventVariables.RemoveAds, OnRemoveAds);
            EventManager.StartListening(EventVariables.BuyVip, OnBuyVip);
            EventManager.StartListening(EventVariables.UpdateMission, CheckNotiMission);
        }

        public override void Show()
        {
            base.Show();

            int chapter = Mathf.Min(Manager.instance.Chapter, Manager.instance.levelDataSO.chapters.Count);
            int stage = Manager.instance.Stage;

            Chapter currentChapter = Manager.instance.levelDataSO.chapters[chapter - 1];

            if (currentChapter.stages[stage - 1] == StageType.Tangle)
            {
                btnPlay.gameObject.SetActive(true);
            }
            else
            {
                btnPlay.gameObject.SetActive(false);
            }

            OnRemoveAds();
            OnBuyVip();

            CheckNotiMission();

            canvasGroup.interactable = false;
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, 0.5f).OnComplete(() =>
            {
                canvasGroup.interactable = true;

                UIManager.instance.ShowUIRate();
            }).SetUpdate(true);

            if (GameplayController.instance.gameMode == GameMode.DailyChallenge)
            {
                OnClickButtonMission();

                GameplayController.instance.gameMode = GameMode.Tangle;
            }
            else
            {
                UIManager.instance.ShowUILevelProgress();
            }

            if (ACEPlay.Bridge.BridgeController.instance.currentLevel < Manager.instance.StageWin)
            {
                ACEPlay.Bridge.BridgeController.instance.currentLevel = Manager.instance.StageWin;
            }
        }

        public override void Hide()
        {
            base.Hide();

            UIManager.instance.UILevelProgress.Hide();

            canvasGroup.interactable = false;
            canvasGroup.DOFade(0f, 0.5f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            }).SetUpdate(true);
        }

        public void CheckNotiMission()
        {
            notiMission.SetActive(MissionManager.instance.CanClaim());
        }

        public void OnClickButtonPlay()
        {
            GameplayController.instance.gameMode = GameMode.Tangle;

            UnityEvent e = new UnityEvent();
            e.AddListener(() =>
            {
                GameplayController.instance.Init();
                Hide();
            });
            
            if (ACEPlay.Bridge.BridgeController.instance.IsShowAdsPlay)
            {
                ACEPlay.Bridge.BridgeController.instance.ShowInterstitial("tangle_play", e);
            }
            else
            {
                e.Invoke();
            }
        }

        public void OnClickButtonHome()
        {
            if (UIManager.instance.UIShop != null)
            {
                UIManager.instance.UIShop.Hide();
            }

            if (UIManager.instance.UIMission != null)
            {
                UIManager.instance.UIMission.Hide();
            }

            int chapter = Mathf.Min(Manager.instance.Chapter, Manager.instance.levelDataSO.chapters.Count);
            int stage = Manager.instance.Stage;
            Chapter currentChapter = Manager.instance.levelDataSO.chapters[chapter - 1];
            if (currentChapter.stages[stage - 1] == StageType.Tangle)
            {
                btnPlay.gameObject.SetActive(true);
            }

            UIManager.instance.ShowUILevelProgress();
            groupButtonTop.SetActive(true);
        }

        public void OnClickButtonShop()
        {
            if (UIManager.instance.UIShop == null)
            {
                if (UIManager.instance.UIMission != null)
                {
                    UIManager.instance.UIMission.Hide();
                }

                UIManager.instance.ShowUIShop(true);
                btnPlay.gameObject.SetActive(false);
                UIManager.instance.UILevelProgress.Hide();

                groupButtonTop.SetActive(false);
            }
        }
        
        public void OnClickButtonMission()
        {
            if (UIManager.instance.UIMission == null)
            {
                if (UIManager.instance.UIShop != null)
                {
                    UIManager.instance.UIShop.Hide();
                }

                UIManager.instance.ShowUIMission();
                btnPlay.gameObject.SetActive(false);
                UIManager.instance.UILevelProgress.Hide();

                groupButtonTop.SetActive(true);
            }
        }

        public void OnClickButtonSetting()
        {
            UIManager.instance.ShowUISetting();
        }

        public void OnClickButtonRemoveAds()
        {
            UnityStringEvent e = new UnityStringEvent();
            e.AddListener(result =>
            {
                ACEPlay.Bridge.BridgeController.instance.CanShowAds = false;
                EventManager.EmitEvent(EventVariables.RemoveAds);

                UIManager.instance.ShowTextNotice("Thank you for your purchase!");
            });
            ACEPlay.Bridge.BridgeController.instance.PurchaseProduct(removeAdsPack.keyIAP, e);
        }

        public void OnClickButtonVip()
        {
            UIManager.instance.ShowUIVipPack();
        }

        public void OnRemoveAds()
        {
            if (!ACEPlay.Bridge.BridgeController.instance.CanShowAds)
            {
                btnRemoveAds.SetActive(false);
            }
        }

        public void OnBuyVip()
        {
            if (Manager.instance.IsVip)
            {
                if (string.IsNullOrEmpty(Manager.instance.DateExpiredVip))
                {
                    btnVip.SetActive(false);
                }
            }
        }
    }
}