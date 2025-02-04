using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace Duc
{
    public class UIShop : UI
    {
        [SerializeField] private ScrollRect scrollShop;
        [SerializeField] private Transform vipParent;
        [SerializeField] private Transform moneyParent;
        [SerializeField] private Transform gemParent;
        [SerializeField] private GameObject vipPack;
        [SerializeField] private GameObject buttonClose;

        private ChildShopRemoveAds childShopRemoveAds;
        public ChildShopGem childShopFreeGem;

        public override void Start()
        {
            base.Start();

            EventManager.StartListening(EventVariables.RemoveAds, OnRemoveAds);
            EventManager.StartListening(EventVariables.BuyVip, OnBuyVip);
        }

        public override void Show()
        {
            base.Show();

            foreach (var data in UIManager.instance.shopDataSO.shopPacks)
            {
                if (data.Name.Contains("Remove Ads"))
                {
                    if (ACEPlay.Bridge.BridgeController.instance.CanShowAds)
                    {
                        childShopRemoveAds = Instantiate(Resources.Load<ChildShopRemoveAds>(Path.Combine("UI", "Child Shop Remove Ads")), vipParent);
                        childShopRemoveAds.Init(data);
                    }
                }
                else if (data.Name.Contains("Money"))
                {
                    var childShopMoney = Instantiate(Resources.Load<ChildShopMoney>(Path.Combine("UI", "Child Shop Money")), moneyParent);
                    childShopMoney.Init(data);
                }
                else if (data.Name.Contains("Gem"))
                {
                    var childShopGem = Instantiate(Resources.Load<ChildShopGem>(Path.Combine("UI", "Child Shop Gem")), gemParent);
                    childShopGem.Init(data);
                }
            }

            childShopFreeGem.Init(null);

            OnBuyVip();

            if (childShopRemoveAds == null && !vipPack.activeSelf)
            {
                vipParent.gameObject.SetActive(false);
            }

            Rebuild();
        }

        public override void Hide()
        {
            base.Hide();

            if (UIManager.instance.UIBuyBooster != null || UIManager.instance.UILose != null)
            {
                ACEPlay.Native.NativeAds.instance.DisplayNativeAds(true);
                ACEPlay.Bridge.BridgeController.instance.ShowBannerCollapsible();
            }
        }

        public void Rebuild()
        {
            StartCoroutine(RebuildLayout());
        }

        private IEnumerator RebuildLayout()
        {
            yield return new WaitForEndOfFrame();

            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollShop.content);
        }

        public void ShowCloseButton()
        {
            buttonClose.SetActive(true);
        }

        public void OnClickVipPack()
        {
            UIManager.instance.ShowUIVipPack();
        }

        public void OnRemoveAds()
        {
            if (!ACEPlay.Bridge.BridgeController.instance.CanShowAds)
            {
                if (childShopRemoveAds != null)
                {
                    childShopRemoveAds.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
                    {
                        Destroy(childShopRemoveAds.gameObject);

                        if (childShopRemoveAds == null && !vipPack.activeSelf)
                        {
                            vipParent.gameObject.SetActive(false);
                        }

                        Rebuild();
                    });
                }
            }
        }

        public void OnBuyVip()
        {
            if (Manager.instance.IsVip && string.IsNullOrEmpty(Manager.instance.DateExpiredVip))
            {
                vipPack.SetActive(false);
            }
        }

        public void OnClickButtonClose()
        {
            Hide();
        }
    }
}