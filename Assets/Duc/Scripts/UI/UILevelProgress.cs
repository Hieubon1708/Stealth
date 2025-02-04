using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Duc
{
    public class UILevelProgress : UI
    {
        [Header("Level Progress")]
        [SerializeField] private RectTransform rtsfParent;
        [SerializeField] private GameObject title;
        [SerializeField] private TextMeshProUGUI txtTitle;
        [SerializeField] private RectTransform rtsfLevelProgress;
        [SerializeField] private Slider levelProgress;
        [SerializeField] private HorizontalLayoutGroup gridProgress;
        [SerializeField] private TextMeshProUGUI txtDescription;

        [Header("Prefab")]
        [SerializeField] private GameObject objTangle;
        [SerializeField] private GameObject objStealthNormal;
        [SerializeField] private GameObject objStealthElite;
        [SerializeField] private GameObject objStealthBonus;
        [SerializeField] private GameObject objStealthBoss;

        private List<GameObject> spawnedStages = new List<GameObject>();

        public override void Start()
        {
            base.Start();
        }

        public override void Show()
        {
            base.Show();

            ShowLevelProgress();
        }

        public override void Hide()
        {
            base.Hide();

            foreach (var stage in spawnedStages)
            {
                stage.transform.DOKill();
                stage.Recycle();
            }

            spawnedStages.Clear();

            gameObject.SetActive(false);
        }

        public void ShowLevelProgress()
        {
            int chapter = Mathf.Min(Manager.instance.Chapter, Manager.instance.levelDataSO.chapters.Count);
            int stage = Manager.instance.Stage;

            txtTitle.text = $"Chapter {Manager.instance.Chapter}";

            Chapter currentChapter = Manager.instance.levelDataSO.chapters[chapter - 1];

            levelProgress.minValue = 1;
            levelProgress.maxValue = currentChapter.stages.Count;
            levelProgress.value = stage;

            float sizeX = Mathf.Min(currentChapter.stages.Count + 1, 6) * 100f;
            rtsfLevelProgress.sizeDelta = new Vector2(sizeX, rtsfLevelProgress.sizeDelta.y);

            gridProgress.spacing = sizeX / (currentChapter.stages.Count - 1);

            switch (currentChapter.stages[stage - 1])
            {
                case StageType.Tangle:
                    txtDescription.text = "Rescue Poppy";
                    break;
                case StageType.StealthNormal:
                    txtDescription.text = "Defeat all enemies";
                    break;
                case StageType.StealthBonus:
                    txtDescription.text = "Collect money";
                    break;
                case StageType.StealthElite:
                    txtDescription.text = "Defeat all enemies";
                    break;
                case StageType.StealthBoss:
                    txtDescription.text = "Defeat the boss";
                    break;
            }
          
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