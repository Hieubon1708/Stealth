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

        private void Awake()
        {
            instance = this;
            RescuedCharacter = new List<Character>() { Character.Bobby };
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

        public List<GameController.WeaponType> WeaponCharacter
        {
            get
            {
                string txt = PlayerPrefs.GetString("WeaponCharacter", string.Empty);
                if (!string.IsNullOrEmpty(txt))
                {
                    return JsonConvert.DeserializeObject<List<GameController.WeaponType>>(txt);
                }
                return new List<GameController.WeaponType>();
            }
            set
            {
                string txt = JsonConvert.SerializeObject(value);
                PlayerPrefs.SetString("WeaponCharacter", txt);
            }
        }

        public List<Character> RescuedCharacter
        {
            get
            {
                string txt = PlayerPrefs.GetString("RescuedCharacter", string.Empty);
                if (!string.IsNullOrEmpty(txt))
                {
                    return JsonConvert.DeserializeObject<List<Character>>(txt);
                }

                return new List<Character>();
            }
            set
            {
                string txt = JsonConvert.SerializeObject(value);
                PlayerPrefs.SetString("RescuedCharacter", txt);
            }
        }

        public enum Character
        {
            Poppy = 0,
            Bobby = 1,
        }

        public void Win()
        {
            RescuedCharacter = GameController.instance.tempPoppies;
            WeaponCharacter = GameController.instance.tempWeaponPoppies;
            //IsStart = GameController.instance.isTempStart;
        }
    }
}
