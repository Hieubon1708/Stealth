using DG.Tweening;
using Duc;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hunter
{
    public class PanelLose : MonoBehaviour
    {
        public RectTransform progress;
        public GameObject objTangle;
        public GameObject objStealthNormal;
        public GameObject objStealthElite;
        public GameObject objStealthBonus;
        public GameObject objStealthBoss;
        public Slider levelProgress;
        public HorizontalLayoutGroup gridProgress;
        private List<GameObject> spawnedStages = new List<GameObject>();

        public void OnEnable()
        {
            ACEPlay.Native.NativeAds.instance.DisplayNativeAds(true);
            ACEPlay.Bridge.BridgeController.instance.ShowBannerCollapsible();
            AudioController.instance.PlaySoundNVibrate(AudioController.instance.lose, 0);
            if (gridProgress.transform.childCount > 0)
            {
                for (int i = 0; i < gridProgress.transform.childCount; i++)
                {
                    Destroy(gridProgress.transform.GetChild(i).gameObject);
                }
            }
            spawnedStages.Clear();
            DOVirtual.DelayedCall(0.1f, delegate
            {
                ShowLevelProgress();
            });
        }

        public void OnDisable()
        {
            ACEPlay.Native.NativeAds.instance.DisplayNativeAds(false);
            ACEPlay.Bridge.BridgeController.instance.HideBannerCollapsible();
        }

        public void ShowLevelProgress()
        {
            int chapter = Mathf.Min(UIController.instance.gamePlay.tempChapter, Manager.instance.levelDataSO.chapters.Count);
            int stage = UIController.instance.gamePlay.tempStage;

            Chapter currentChapter = Manager.instance.levelDataSO.chapters[chapter - 1];

            levelProgress.minValue = 1;
            levelProgress.maxValue = currentChapter.stages.Count;
            levelProgress.value = stage;

            float sizeX = Mathf.Min(currentChapter.stages.Count + 1, 6) * 100f;
            progress.sizeDelta = new Vector2(sizeX, progress.sizeDelta.y);

            gridProgress.spacing = sizeX / (currentChapter.stages.Count - 1);

            foreach (var stageType in currentChapter.stages)
            {
                GameObject newObj;
                switch (stageType)
                {
                    case StageType.Tangle:
                        newObj = objTangle.Spawn(gridProgress.transform);
                        newObj.transform.localScale = Vector3.one;
                        spawnedStages.Add(newObj);
                        break;
                    case StageType.StealthNormal:
                        newObj = objStealthNormal.Spawn(gridProgress.transform);
                        newObj.transform.localScale = Vector3.one;
                        spawnedStages.Add(newObj);
                        break;
                    case StageType.StealthElite:
                        newObj = objStealthElite.Spawn(gridProgress.transform);
                        newObj.transform.localScale = Vector3.one;
                        spawnedStages.Add(newObj);
                        break;
                    case StageType.StealthBoss:
                        newObj = objStealthBoss.Spawn(gridProgress.transform);
                        newObj.transform.localScale = Vector3.one;
                        spawnedStages.Add(newObj);
                        break;
                    case StageType.StealthBonus:
                        newObj = objStealthBonus.Spawn(gridProgress.transform);
                        newObj.transform.localScale = Vector3.one;
                        spawnedStages.Add(newObj);
                        break;
                }
            }

            for (int i = 0; i < spawnedStages.Count; i++)
            {
                spawnedStages[i].transform.localScale = Vector3.one;

                if (i < (stage - 1))
                {
                    spawnedStages[i].GetComponentInChildren<Image>().color = Color.green;
                }
                else if (i == (stage - 1))
                {
                    spawnedStages[i].GetComponentInChildren<Image>().color = Color.yellow;
                }
                else
                {
                    spawnedStages[i].GetComponentInChildren<Image>().color = Color.white;
                }
            }

            spawnedStages[stage - 1].transform.DOScale(1.2f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
        }
    }
}
