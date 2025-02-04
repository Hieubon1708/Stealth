using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TigerForge;
using TMPro;
using UnityEngine;

namespace Duc
{
    public class UIVipPack : UI
    {
        [SerializeField] private TextMeshProUGUI txtPrice7days;
        [SerializeField] private TextMeshProUGUI txtPriceForever;
        [SerializeField] private GameObject objCountDownExpired;
        [SerializeField] private TextMeshProUGUI txtCountDownExpired;

        private ShopPack vipPack7Days;
        private ShopPack vipPackForever;

        private void Awake()
        {
            foreach (var shopPack in UIManager.instance.shopDataSO.shopPacks)
            {
                if (shopPack.Name.Contains("Vip"))
                {
                    if (shopPack.Name.Contains("Days"))
                    {
                        vipPack7Days = shopPack;
                    }
                    else if (shopPack.Name.Contains("Forever"))
                    {
                        vipPackForever = shopPack;
                    }
                }
            }
        }

        public override void Start()
        {
            base.Start();
            EventManager.StartListening(EventVariables.BuyVip, CheckDateExpire);
        }

        public override void Show()
        {
            base.Show();

            if (!string.IsNullOrEmpty(vipPack7Days.keyIAP))
            {
                string localizedPrice = Manager.instance.GetProductPrice(vipPack7Days.keyIAP);
                if (!string.IsNullOrEmpty(localizedPrice))
                {
                    txtPrice7days.text = localizedPrice;
                }
                else
                {
                    txtPrice7days.text = string.Format("$ {0}", vipPack7Days.price);
                }
            }
            else
            {
                txtPrice7days.text = string.Empty;
            }
            
            if (!string.IsNullOrEmpty(vipPackForever.keyIAP))
            {
                string localizedPrice = Manager.instance.GetProductPrice(vipPackForever.keyIAP);
                if (!string.IsNullOrEmpty(localizedPrice))
                {
                    txtPriceForever.text = localizedPrice;
                }
                else
                {
                    txtPriceForever.text = string.Format("$ {0}", vipPackForever.price);
                }
            }
            else
            {
                txtPriceForever.text = string.Empty;
            }

            CheckDateExpire();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void CheckDateExpire()
        {
            if (Manager.instance.IsVip)
            {
                string dateExpiredStr = Manager.instance.DateExpiredVip;
                if (!string.IsNullOrEmpty(dateExpiredStr))
                {
                    objCountDownExpired.SetActive(true);

                    if (countDownExpired != null)
                    {
                        StopCoroutine(countDownExpired);
                    }

                    countDownExpired = StartCoroutine(CountDownExpired(() =>
                    {
                        Manager.instance.IsVip = false;
                        Manager.instance.DateExpiredVip = null;

                        CheckDateExpire();
                    }));
                }
            }
            else
            {
                objCountDownExpired.SetActive(false);
            }
        }

        Coroutine countDownExpired = null;
        IEnumerator CountDownExpired(Action onDone = null)
        {
            string strDateExpired = Manager.instance.DateExpiredVip;

            if (DateTime.TryParseExact(strDateExpired, MyTime.instance.format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateExpired))
            {
                TimeSpan timeLeft;

                while (DateTime.Compare(dateExpired, MyTime.instance.GetCurrentTime()) > 0)
                {
                    timeLeft = dateExpired.Subtract(MyTime.instance.GetCurrentTime());

                    if (timeLeft.Days > 0)
                    {
                        txtCountDownExpired.text = string.Format("Expired in:\n{0}d {1:00}h {2:00}m", timeLeft.Days, timeLeft.Hours, timeLeft.Minutes);
                    }
                    else
                    {
                        txtCountDownExpired.text = string.Format("Expired in:\n{0:00}:{1:00}:{2:00}", timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
                    }

                    yield return null;
                }

                if (onDone != null) onDone.Invoke();

                countDownExpired = null;
            }
            else
            {
                Debug.LogError(string.Format("Can't convert DateTime {0}\nAt CountDownExpired; from UIVipPack.cs", strDateExpired));
            }
        }

        public void OnClickButtonBuy7Days()
        {
            if (Manager.instance.IsVip) return;

            UnityStringEvent e = new UnityStringEvent();
            e.AddListener(result =>
            {
                DateTime currentDate = MyTime.instance.GetCurrentTime(true);
                currentDate = currentDate.AddDays(7);

                Manager.instance.DateExpiredVip = MyTime.instance.GetStrTime(currentDate);
                Manager.instance.IsVip = true;

                EventManager.EmitEvent(EventVariables.BuyVip);

                UIManager.instance.ShowTextNotice("Thank you for your purchase!");
            });
            ACEPlay.Bridge.BridgeController.instance.PurchaseProduct(vipPack7Days.keyIAP, e);
        }

        public void OnClickButtonBuyForever()
        {
            UnityStringEvent e = new UnityStringEvent();
            e.AddListener(result =>
            {
                Manager.instance.IsVip = true;
                Manager.instance.DateExpiredVip = null;

                EventManager.EmitEvent(EventVariables.BuyVip);

                UIManager.instance.ShowTextNotice("Thank you for your purchase!");

                Hide();
            });
            ACEPlay.Bridge.BridgeController.instance.PurchaseProduct(vipPackForever.keyIAP, e);
        }

        public void OnClickButtonClose()
        {
            Hide();
        }
    }
}