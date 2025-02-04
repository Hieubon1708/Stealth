using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duc
{
    public class ChildShopGem : MonoBehaviour
    {
        [SerializeField] private Image imgIcon;
        [SerializeField] private TextMeshProUGUI txtValue;
        [SerializeField] private TextMeshProUGUI txtPrice;

        [SerializeField] private GameObject countDownFree;
        [SerializeField] private TextMeshProUGUI txtCountDownFree;
        [SerializeField] private GameObject video;

        public bool isFree = false;

        private ShopPack data;

        public void Init(ShopPack _data)
        {
            if (!isFree)
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
            else
            {
                CheckFreeGem();
            }
        }

        public void CheckFreeGem()
        {
            string strDateClaim = Manager.instance.DateClaimFreeGem;
            if (!string.IsNullOrEmpty(strDateClaim))
            {
                countDownFree.SetActive(true);
                video.SetActive(false);

                if (countDownFreeGem != null)
                {
                    StopCoroutine(countDownFreeGem);
                }

                countDownFreeGem = StartCoroutine(CountDownToNextFreeGem(() =>
                {
                    Manager.instance.DateClaimFreeGem = null;
                    countDownFree.SetActive(false);
                    video.SetActive(true);
                }));
            }
        }

        Coroutine countDownFreeGem = null;
        IEnumerator CountDownToNextFreeGem(Action onDone = null)
        {
            string strDateClaim = Manager.instance.DateClaimFreeGem;
            if (!string.IsNullOrEmpty(strDateClaim))
            {
                if (DateTime.TryParseExact(strDateClaim, MyTime.instance.format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateClaim))
                {
                    DateTime nextDay = dateClaim.AddHours(1);
                    TimeSpan timeLeft;

                    while (DateTime.Compare(MyTime.instance.GetCurrentTime(), nextDay) < 0)
                    {
                        timeLeft = nextDay.Subtract(MyTime.instance.GetCurrentTime());
                        txtCountDownFree.text = string.Format("Free in:\n{0:00}:{1:00}:{2:00}", timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);

                        yield return null;
                    }

                    if (onDone != null) onDone.Invoke();

                    countDownFreeGem = null;
                }
                else
                {
                    Debug.LogError(string.Format("Can't convert DateTime {0}\nAt CountDownToNextFreeGem; from ChildShopGem.cs", strDateClaim));
                }
            }
        }

        public void OnClick()
        {
            if (!isFree)
            {
                UnityStringEvent e = new UnityStringEvent();
                e.AddListener(result =>
                {
                    UIEconomy.instance.AddGem(data.rewards[0].value, imgIcon.transform);

                    UIManager.instance.ShowTextNotice("Thank you for your purchase!");
                });
                ACEPlay.Bridge.BridgeController.instance.PurchaseProduct(data.keyIAP, e);
            }
            else
            {
                string strDateClaimFreeGem = Manager.instance.DateClaimFreeGem;
                if (!string.IsNullOrEmpty(strDateClaimFreeGem))
                {
                    if (DateTime.TryParseExact(strDateClaimFreeGem, MyTime.instance.format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateClaim))
                    {
                        DateTime currentDate = MyTime.instance.GetCurrentTime();
                        TimeSpan diffDate = currentDate.Subtract(dateClaim);

                        if (diffDate.Hours < 1)
                        {
                            return;
                        }
                    }
                    else
                    {
                        Debug.LogError(string.Format("Can't convert DateTime {0}\nAt OnClick; from ChildShopGem.cs", strDateClaimFreeGem));
                    }
                }

                if (ACEPlay.Bridge.BridgeController.instance.IsRewardReady())
                {
                    UnityEvent e = new UnityEvent();
                    e.AddListener(() =>
                    {
                        UIEconomy.instance.AddGem(10, imgIcon.transform);
                        Manager.instance.DateClaimFreeGem = MyTime.instance.GetCurrentTimeStr();
                        CheckFreeGem();
                    });
                    ACEPlay.Bridge.BridgeController.instance.ShowRewarded("gem_shop", e);
                }
                else
                {
                    UIManager.instance.ShowTextNotice("Ad is not ready!");
                }
            }
        }
    }
}