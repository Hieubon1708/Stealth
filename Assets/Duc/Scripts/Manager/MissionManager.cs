using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using TigerForge;
using UnityEngine;

namespace Duc
{
    public class MissionManager : MonoBehaviour
    {
        public static MissionManager instance;

        public MissionDataSO missionDataSO;

        private void Awake()
        {
            if (instance == null) instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            Init();

            EventManager.StartListening(EventVariables.UpdateMission, UpdateMission);
        }

        public List<Mission> Missions
        {
            get
            {
                string txt = PlayerPrefs.GetString("Mission", string.Empty);
                if (!string.IsNullOrEmpty(txt))
                {
                    return JsonConvert.DeserializeObject<List<Mission>>(txt);
                }

                return new List<Mission>();
            }
            set
            {
                string txt = JsonConvert.SerializeObject(value);
                PlayerPrefs.SetString("Mission", txt);
            }
        }
        
        public int LevelTwistedChallenge
        {
            get
            {
                return PlayerPrefs.GetInt("LevelTwistedChallenge", 1);
            }
            set
            {
                PlayerPrefs.SetInt("LevelTwistedChallenge", value);
            }
        }

        public int TwistedChallengePlayCount
        {
            get
            {
                return PlayerPrefs.GetInt("TwistedChallengePlayCount", 0);
            }
            set
            {
                PlayerPrefs.SetInt("TwistedChallengePlayCount", value);
            }
        }

        public string DateRandomMission
        {
            get
            {
                return PlayerPrefs.GetString("DateRandomMission", string.Empty);
            }
            set
            {
                PlayerPrefs.SetString("DateRandomMission", value);
            }
        }

        public void Init()
        {
            string strDateRandomMission = DateRandomMission;
            if (!string.IsNullOrEmpty(strDateRandomMission))
            {
                if (DateTime.TryParseExact(strDateRandomMission, MyTime.instance.format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateRandom))
                {
                    if (MyTime.instance.CheckNewDay(dateRandom))
                    {
                        RandomMission();
                        TwistedChallengePlayCount = 0;
                    }
                }
                else
                {
                    Debug.LogError(string.Format("Can't convert DateTime {0}\nAt Init; from MissionManager.cs", strDateRandomMission));
                }
            }
            else
            {
                RandomMission();
                TwistedChallengePlayCount = 0;
            }
        }

        public void UpdateMission()
        {
            var missionData = EventManager.GetDataGroup(EventVariables.UpdateMission);
            MissionType missionType = (MissionType)missionData[0].ToInt();
            int amount = missionData[1].ToInt();

            List<Mission> currentMission = Missions;

            foreach (Mission mission in currentMission)
            {
                if (mission.type == missionType)
                {
                    mission.current += amount;
                }
            }

            Missions = currentMission;
        }

        public void RandomMission()
        {
            List<Mission> missionData = new List<Mission>(missionDataSO.missionDatas);

            List<Mission> newMissions = new List<Mission>();
            for (int i = 0; i < 4; i++)
            {
                Mission random = missionData[UnityEngine.Random.Range(0, missionData.Count)];

                while (IsMissionTypeExist(newMissions, random))
                {
                    random = missionData[UnityEngine.Random.Range(0, missionData.Count)];
                }

                missionData.Remove(random);
                newMissions.Add(random);
            }

            newMissions.Sort((o1, o2) => o1.reward.value.CompareTo(o2.reward.value));

            Missions = newMissions;

            DateRandomMission = MyTime.instance.GetCurrentTimeStr(true);
        }

        private bool IsMissionTypeExist(List<Mission> missions, Mission mission)
        {
            foreach (var item in missions)
            {
                if (item.type == mission.type)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CanClaim()
        {
            foreach (var mission in Missions)
            {
                if (!mission.claimed && mission.current >= mission.amount)
                {
                    return true;
                }
            }

            return false;
        }
    }
}