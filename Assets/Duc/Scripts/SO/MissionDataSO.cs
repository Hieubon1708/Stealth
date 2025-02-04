using System.Collections.Generic;
using UnityEngine;

namespace Duc
{
    [CreateAssetMenu(fileName = "MissionData", menuName = "ScriptableObjects/MissionData")]
    public class MissionDataSO : ScriptableObject
    {
        public List<Mission> missionDatas = new List<Mission>();
    }

    [System.Serializable]
    public class Mission
    {
        public MissionType type;
        public int amount;
        public int current;
        public Reward reward;
        public bool claimed;
    }

    public enum MissionType
    {
        KillEnemy = 0,
        KillBoss = 1,
        RescuePoppy = 2,
        CollectMoney = 3,
        UseBooster = 4,
        WatchAds = 5
    }
}