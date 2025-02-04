using DG.Tweening;
using Duc.PoppyTangle;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Duc
{
    public class UIMission : UI
    {
        [SerializeField] private ScrollRect scrollMission;
        private List<ChildMission> childMissions = new List<ChildMission>();

        [Header("Daily Challenge")]
        [SerializeField] private List<Image> imgIconLevels;
        [SerializeField] private Slider progressDailyChallenge;
        [SerializeField] private TextMeshProUGUI txtProgressDailyChallenge;
        [SerializeField] private ButtonScale btnPlayChallenge;

        public override void Start()
        {
            base.Start();
        }

        public override void Show()
        {
            base.Show();

            int levelTwistedChallenge = MissionManager.instance.LevelTwistedChallenge;
            int challengePlayCount = MissionManager.instance.TwistedChallengePlayCount;

            progressDailyChallenge.value = challengePlayCount / 3f;
            txtProgressDailyChallenge.text = $"{challengePlayCount} / 3";

            if (levelTwistedChallenge < Manager.instance.TotalLevelTwistedChallenge)
            {
                for (int i = 0; i < 3; i++)
                {
                    int index = levelTwistedChallenge + i;

                    if (index <= Manager.instance.TotalLevelTwistedChallenge)
                    {
                        imgIconLevels[i].sprite = Resources.Load<Sprite>(Path.Combine("IconTwistedChallenge", index.ToString()));
                    }
                    else
                    {
                        imgIconLevels[i].sprite = Resources.Load<Sprite>(Path.Combine("IconTwistedChallenge", Random.Range(1, Manager.instance.TotalLevelTwistedChallenge + 1).ToString()));
                    }
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    imgIconLevels[i].sprite = Resources.Load<Sprite>(Path.Combine("IconTwistedChallenge", Random.Range(1, Manager.instance.TotalLevelTwistedChallenge + 1).ToString()));
                }
            }

            btnPlayChallenge.Active_Interactable(challengePlayCount < 3);

            List<Mission> missions = MissionManager.instance.Missions;

            foreach (Mission mission in missions)
            {
                var newChildMission = Instantiate(Resources.Load<ChildMission>(Path.Combine("UI", "Child Mission")), scrollMission.content);
                newChildMission.Init(mission);
                childMissions.Add(newChildMission);
            }

            foreach (var child in childMissions)
            {
                if (child.mission.claimed)
                {
                    child.transform.SetAsLastSibling();
                }
                else if (child.mission.current >= child.mission.amount)
                {
                    child.transform.SetAsFirstSibling();
                }
            }

            float delay = 0f;
            foreach (Transform child in scrollMission.content)
            {
                child.transform.localScale = Vector3.zero;
                child.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(delay).SetUpdate(true);
                delay += 0.05f;
            }
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void OnClickButtonPlayDailyChallenge()
        {
            if (MissionManager.instance.LevelTwistedChallenge > Manager.instance.TotalLevelTwistedChallenge)
            {
                UIManager.instance.ShowTextNotice("Coming soon!");
                return;
            }

            GameplayController.instance.gameMode = GameMode.DailyChallenge;

            int chapter = Mathf.Min(Manager.instance.Chapter, Manager.instance.levelDataSO.chapters.Count);
            int stage = Manager.instance.Stage;

            Chapter currentChapter = Manager.instance.levelDataSO.chapters[chapter - 1];

            if (currentChapter.stages[stage - 1] == StageType.Tangle)
            {
                FadeManager.instance.Fade(() =>
                {
                    UIManager.instance.UIHome.Hide();
                    Hide();

                    GameplayController.instance.Init();
                });
            }
            else
            {
                FadeManager.instance.UnloadSceneStealth(() =>
                {
                    UIManager.instance.UIHome.Hide();
                    Hide();
                    GameplayController.instance.SetUp();
                    GameplayController.instance.Init();
                });
            }
        }
    }
}