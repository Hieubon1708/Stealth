using UnityEngine;
using Newtonsoft.Json;
using Duc;
using System.Collections.Generic;
using System.IO;

namespace Hunter
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public PoppyConfig[] poppyConfig;
        public TotalLevel totalLevel;

        private void Awake()
        {
            instance = this;
            /*string path = "Assets/HieuBon/Resources";
            string[] prefabsFiles = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
            TotalLevel totalLevel = new TotalLevel();
            totalLevel.levelNames = new List<string>();
            for (int i = 0; i < prefabsFiles.Length; i++)
            {
                string name = prefabsFiles[i].Replace(path, "").Replace(".prefab", "").Replace("\\", "");
                totalLevel.levelNames.Add(name);
            }
            string js = JsonConvert.SerializeObject(totalLevel);
            Debug.Log(js);
            string p = Path.Combine(Application.dataPath, "HieuBon/Resources/TotalLevel.json");
            File.WriteAllText(p, js);*/
            var data = Resources.Load<TextAsset>("TotalLevel");
            totalLevel = JsonConvert.DeserializeObject<TotalLevel>(data.text);
        }

        public string Level
        {
            get
            {
                int chapter = Mathf.Min(Manager.instance.Chapter - 1, Manager.instance.levelDataSO.chapters.Count - 1);
                int stage = Manager.instance.Stage - 1;
                return chapter + " " + stage;
            }
        }

        public int LevelStealk
        {
            get
            {
                return totalLevel.levelNames.IndexOf(Level) + 1;
            }
        }

        public bool IsStart
        {
            get
            {
                return PlayerPrefs.GetInt("IsStart", 0) == 1;
            }
            set
            {
                PlayerPrefs.SetInt("IsStart", value ? 1 : 0);
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
            Bubba = 2,
            Catnap = 3,
            CraftyCorn = 4,
            Hoppy = 5,
            Kickin = 6,
            PickyPiggy = 7,
            None = 8
        }

        public void Win()
        {
            RescuedCharacter = GameController.instance.tempPoppies;
            WeaponCharacter = GameController.instance.tempWeaponPoppies;
            IsStart = GameController.instance.isTempStart;
        }
    }

    [System.Serializable]
    public class PoppyConfig
    {
        public Poppy[] poppies;
    }

    [System.Serializable]
    public class Poppy
    {
        public GameController.PoppyType poppyType;
        public GameController.WeaponType weaponType;
    }

    public struct TotalLevel
    {
        public List<string> levelNames;
    }
}
