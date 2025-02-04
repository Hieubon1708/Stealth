using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Duc
{
    public class ChildMission : MonoBehaviour
    {
        [HideInInspector] public Mission mission;

        [SerializeField] private List<TextMeshProUGUI> txtsTitle;
        [SerializeField] private Transform trsfReward;
        [SerializeField] private List<TextMeshProUGUI> txtsRewardAmount;
        [SerializeField] private List<Image> imgsIcon;
        [SerializeField] private Slider progress;
        [SerializeField] private TextMeshProUGUI txtProgress;

        [SerializeField] private GameObject objClaimable;
        [SerializeField] private GameObject objClaimed;

        [SerializeField] private Sprite[] sprIcons;

        public void Init(Mission data)
        {
            mission = data;

            switch (mission.type)
            {
                case MissionType.KillEnemy:
                    txtsTitle[0].text = $"Kill <color=#FFC61A>{mission.amount}</color> Enemies";
                    txtsTitle[1].text = $"Kill {mission.amount} Enemies";
                    break;
                case MissionType.KillBoss:
                    txtsTitle[0].text = $"Kill <color=#FFC61A>{mission.amount}</color> Boss";
                    txtsTitle[1].text = $"Kill {mission.amount} Boss";
                    break;
                case MissionType.RescuePoppy:
                    txtsTitle[0].text = $"Rescue <color=#FFC61A>{mission.amount}</color> Poppy";
                    txtsTitle[1].text = $"Rescue {mission.amount} Poppy";
                    break;
                case MissionType.CollectMoney:
                    txtsTitle[0].text = $"Collect <color=#FFC61A>{mission.amount}</color> Money";
                    txtsTitle[1].text = $"Collect {mission.amount} Money";
                    break;
                case MissionType.UseBooster:
                    txtsTitle[0].text = $"Use Booster <color=#FFC61A>{mission.amount}</color> times";
                    txtsTitle[1].text = $"Use Booster {mission.amount} times";
                    break;
                case MissionType.WatchAds:
                    txtsTitle[0].text = $"Watch Rewarded Ads <color=#FFC61A>{mission.amount}</color> times";
                    txtsTitle[1].text = $"Watch Rewarded Ads {mission.amount} times";
                    break;
            }

            imgsIcon.ForEach(img => img.sprite = sprIcons[(int)mission.type]);
            txtsRewardAmount.ForEach(txt => txt.text = mission.reward.value.ToString());
            progress.value = mission.current / (float) mission.amount;
            txtProgress.text = $"{mission.current}/{mission.amount}";

            if (mission.claimed)
            {
                objClaimed.SetActive(true);
            }
            else if (mission.current >= mission.amount)
            {
                objClaimable.SetActive(true);
            }
        }

        public void OnClick()
        {
            if (!mission.claimed && mission.current >= mission.amount)
            {
                List<Mission> currentMission = MissionManager.instance.Missions;
                foreach (Mission data in currentMission)
                {
                    if (data.type == mission.type && data.amount == mission.amount)
                    {
                        data.claimed = true;
                        break;
                    }
                }
                MissionManager.instance.Missions = currentMission;

                mission.claimed = true;

                UIEconomy.instance.AddCash(mission.reward.value, trsfReward);

                objClaimable.SetActive(false);
                objClaimed.SetActive(true);

                transform.SetAsLastSibling();

                UIManager.instance.UIHome.CheckNotiMission();
            }
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }
    }
}