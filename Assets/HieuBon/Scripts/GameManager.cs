using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Hunter
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public const int totalLevel = 17;

        public LevelDataSO levelDataSO;

        private void Awake()
        {
            instance = this;
        }

        public enum StageType
        {
            StealthNormal = 0,
            StealthElite = 1,
            StealthBoss = 2,
            StealthBonus = 3
        }

        public int Money
        {
            get
            {
                return PlayerPrefs.GetInt("Money", 0);
            }
            set
            {
                PlayerPrefs.SetInt("Money", Mathf.Max(0, value));
            }
        }

        public int Level
        {
            get
            {
                return PlayerPrefs.GetInt("Level", 1);
            }
            set
            {
                PlayerPrefs.SetInt("Level", value);
            }
        }

        public bool IsFinishedLevel
        {
            get
            {
                return PlayerPrefs.GetInt("IsFinishedLevel", 0) == 1;
            }
            set
            {
                PlayerPrefs.SetInt("IsFinishedLevel", value ? 1 : 0);
            }
        }

        public enum Character
        {
            Male = 0,
            Female = 1,
        }

        public void Win()
        {
        }
    }
}
