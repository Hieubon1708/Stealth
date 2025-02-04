using ACEPlay.Bridge;
using DG.Tweening;
using System.Collections.Generic;
using TigerForge;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Hunter
{
    public class PanelWin : MonoBehaviour
    {
        public TextMeshProUGUI textDollar;
        public TextMeshProUGUI x;

        public Image iconDollar;

        public RectTransform[] dollarChildren;
        public RectTransform arrow;

        public Vector2[] dirs;
        public float[] distances;
        int mul;
        int dollarAward = 30;

        public void OnEnable()
        {
            ACEPlay.Native.NativeAds.instance.DisplayNativeAds(true);
            ACEPlay.Bridge.BridgeController.instance.ShowBannerCollapsible();
            AudioController.instance.PlaySoundNVibrate(AudioController.instance.win, 0);
            textDollar.text = 0.ToString();
            textDollar.color = Color.white;
            iconDollar.enabled = true;
        }

        public void OnDisable()
        {
            arrow.DOKill();
            ACEPlay.Native.NativeAds.instance.DisplayNativeAds(false);
            BridgeController.instance.HideBannerCollapsible();
        }

        public void Watch()
        {
            UnityEvent e = new UnityEvent();
            e.AddListener(() =>
            {
                dollarAward *= mul;
                textDollar.text = dollarAward.ToString();
                TextDollarReduce();
            });
            BridgeController.instance.ShowRewarded("x" + mul + "dollar", e);
        }

        public void TextDollarIncrease()
        {
            textDollar.transform.DOScale(1.35f, 0.1f).SetEase(Ease.Linear);
            textDollar.transform.DOScale(1f, 0.1f).SetEase(Ease.Linear).SetDelay(0.5f);
            DOVirtual.Int(0, dollarAward, 0.25f, (d) =>
            {
                textDollar.text = d.ToString();
            });
        }

        public void TextDollarReduce()
        {
            UIController.instance.layerCover.raycastTarget = true;
            AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);
            arrow.DOKill();
            /*List<float> tempDistances = new List<float>(distances);
            List<float> randomDistances = new List<float>();
            while (tempDistances.Count > 0)
            {
                int indexRandom = Random.Range(0, tempDistances.Count);
                randomDistances.Add(tempDistances[indexRandom]);
                tempDistances.RemoveAt(indexRandom);
            }*/
            /*EventManager.SetDataGroup(EventVariables.UpdateMission, MissionType.CollectMoney, dollarAward);
            EventManager.EmitEvent(EventVariables.UpdateMission);*/
            textDollar.transform.DOScale(1.35f, 0.1f).SetEase(Ease.Linear);
            textDollar.transform.DOScale(1f, 0.1f).SetEase(Ease.Linear).SetDelay(0.5f);
            DOVirtual.Int(dollarAward, 0, 0.25f, (c) =>
            {
                textDollar.text = c.ToString();
            }).OnComplete(delegate
            {
                //UIEconomy.instance.AddCash(dollarAward, iconDollar.transform);
                iconDollar.enabled = false;
                textDollar.DOFade(0f, 0.1f).SetEase(Ease.Linear);
                /*for (int i = 0; i < dollarChildren.Length; i++)
                {
                    int index = i;
                    dollarChildren[index].DOLocalMove(dollarChildren[index].anchoredPosition + randomDistances[index] * dirs[index] * 35, 0.25f).OnComplete(delegate
                    {
                        dollarChildren[index].DOMove(dollarTarget.position, 0.25f).SetEase(Ease.Linear).SetDelay(Random.Range(0.05f, 0.35f));
                    });
                }*/
            });
            DOVirtual.DelayedCall(2.75f, delegate
            {
                UIController.instance.ChangeMap();
            });
        }

        public void LaunchProgress()
        {
            arrow.DOLocalMoveX(380f, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).OnUpdate(delegate
            {
                if (arrow.anchoredPosition.x >= -86 && arrow.anchoredPosition.x <= 86)
                {
                    mul = 5;
                    x.text = "x5";
                }
                else if (arrow.anchoredPosition.x >= -202 && arrow.anchoredPosition.x < -86 || arrow.anchoredPosition.x <= 202 && arrow.anchoredPosition.x > 86)
                {
                    mul = 4;
                    x.text = "x4";
                }
                else if (arrow.anchoredPosition.x >= -321 && arrow.anchoredPosition.x < -202 || arrow.anchoredPosition.x <= 321 && arrow.anchoredPosition.x > 202)
                {
                    mul = 3;
                    x.text = "x3";
                }
                else
                {
                    mul = 2;
                    x.text = "x2";
                }
            });
        }
    }
}