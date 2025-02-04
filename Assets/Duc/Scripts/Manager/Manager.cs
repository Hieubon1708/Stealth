using Newtonsoft.Json;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

namespace Duc
{
    public class Manager : MonoBehaviour
    {
        public static Manager instance;

        public LevelDataSO levelDataSO;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

#if UNITY_EDITOR
            if (ACEPlay.Bridge.BridgeController.instance == null)
            {
                SceneManager.LoadScene(0);
                return;
            }
#endif

            ACEPlay.Bridge.BridgeController.instance.isLoadGameSuccess = true;
            ACEPlay.Bridge.BridgeController.instance.ShowBanner();
        }

        public string GetProductPrice(string IAPKey)
        {
            foreach (var product in ACEPlay.Bridge.BridgeController.instance.availableItemsIAP)
            {
                if (IAPKey.Equals(product.productIdIAP))
                {
                    if (!string.IsNullOrEmpty(product.localizedPriceString) && !string.IsNullOrEmpty(product.localizedCurrencyCode))
                    {
                        return product.localizedPriceString;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return null;
        }

        #region Player Data
        public bool RatedGame
        {
            get
            {
                return PlayerPrefs.GetInt("RatedGame", 0) == 1 ? true : false;
            }
            set
            {
                PlayerPrefs.SetInt("RatedGame", value ? 1 : 0);
            }
        }

        public bool RateShowed
        {
            get
            {
                return PlayerPrefs.GetInt("RateShowed", 0) == 1 ? true : false;
            }
            set
            {
                PlayerPrefs.SetInt("RateShowed", value ? 1 : 0);
            }
        }

        public int StageWin
        {
            get
            {
                return PlayerPrefs.GetInt("StageWin", 1);
            }
            set
            {
                PlayerPrefs.SetInt("StageWin", value);

                RateShowed = false;
            }
        }

        public int TotalLevel
        {
            get
            {
                return 34;
            }
        }
        
        public int TotalLevelTwistedChallenge
        {
            get
            {
                return 7;
            }
        }

        public int Chapter
        {
            get
            {
                return PlayerPrefs.GetInt("Chapter", 1);
            }
            set
            {
                PlayerPrefs.SetInt("Chapter", value);
            }
        }
        
        public int Stage
        {
            get
            {
                return PlayerPrefs.GetInt("Stage", 1);
            }
            set
            {
                int chapter = Mathf.Min(Chapter, levelDataSO.chapters.Count);

                if (value > levelDataSO.chapters[chapter - 1].stages.Count)
                {
                    Chapter++;
                    value = 1;
                }

                PlayerPrefs.SetInt("Stage", value);

                StageWin++;
            }
        }

        

        public Character CurrentCharacterToRescue
        {
            get 
            {
                return (Character)PlayerPrefs.GetInt("CurrentCharacterToRescue", (int)Character.Poppy);
            }
            set
            {
                PlayerPrefs.SetInt("CurrentCharacterToRescue", (int)value);
            }
        }

        public int LevelPoppyTangle
        {
            get
            {
                return PlayerPrefs.GetInt("LevelPoppyTangle", 1);
            }
            set
            {
                PlayerPrefs.SetInt("LevelPoppyTangle", value);
            }
        }
        
        public bool IsVip
        {
            get
            {
                return PlayerPrefs.GetInt("VIP", 0) == 0 ? false : true;
            }
            set
            {
                PlayerPrefs.SetInt("VIP", value ? 1 : 0);
            }
        }

        public string DateExpiredVip
        {
            get
            {
                return PlayerPrefs.GetString("DateExpiredVip", string.Empty);
            }
            set
            {
                PlayerPrefs.SetString("DateExpiredVip", value);
            }
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

        public int TotalEarn
        {
            get
            {
                return PlayerPrefs.GetInt("TotalEarn", 0);
            }
            set
            {
                PlayerPrefs.SetInt("TotalEarn", value);
            }
        }
        
        public int TotalSpend
        {
            get
            {
                return PlayerPrefs.GetInt("TotalSpend", 0);
            }
            set
            {
                PlayerPrefs.SetInt("TotalSpend", value);
            }
        }
        
        public int Gem
        {
            get
            {
                return PlayerPrefs.GetInt("Gem", 0);
            }
            set
            {
                PlayerPrefs.SetInt("Gem", Mathf.Max(0, value));
            }
        }
        
        public int Axe
        {
            get
            {
                return PlayerPrefs.GetInt("Axe", 0);
            }
            set
            {
                PlayerPrefs.SetInt("Axe", Mathf.Max(0, value));
            }
        }
        
        public int FreezeTime
        {
            get
            {
                return PlayerPrefs.GetInt("FreezeTime", 0);
            }
            set
            {
                PlayerPrefs.SetInt("FreezeTime", Mathf.Max(0, value));
            }
        }
        
        public int Hammer
        {
            get
            {
                return PlayerPrefs.GetInt("Hammer", 0);
            }
            set
            {
                PlayerPrefs.SetInt("Hammer", Mathf.Max(0, value));
            }
        }

        public string DateClaimFreeGem
        {
            get
            {
                return PlayerPrefs.GetString("DateClaimFreeGem", string.Empty);
            }
            set
            {
                PlayerPrefs.SetString("DateClaimFreeGem", value);
            }
        }
        #endregion

        #region Player Setting
        public bool IsMuteSound
        {
            get
            {
                return PlayerPrefs.GetInt("IsMuteSound", 0) == 0 ? false : true;
            }
            set
            {
                PlayerPrefs.SetInt("IsMuteSound", value ? 1 : 0);
            }
        }
        public bool IsMuteMusic
        {
            get
            {
                return PlayerPrefs.GetInt("IsMuteMusic", 0) == 0 ? false : true;
            }
            set
            {
                PlayerPrefs.SetInt("IsMuteMusic", value ? 1 : 0);
            }
        }
        public bool IsOffVibration
        {
            get
            {
                return PlayerPrefs.GetInt("IsOffVibration", 0) == 0 ? false : true;
            }
            set
            {
                PlayerPrefs.SetInt("IsOffVibration", value ? 1 : 0);
            }
        }

        public bool SetSound()
        {
            IsMuteSound = !IsMuteSound;
            AudioController.instance.SetMuteSounds();
            return IsMuteSound;
        }

        public bool SetMusic()
        {
            IsMuteMusic = !IsMuteMusic;
            AudioController.instance.SetMuteMusic(true);
            EventManager.EmitEvent(EventVariables.SetMusic);
            return IsMuteMusic;
        }

        public bool SetVibration()
        {
            IsOffVibration = !IsOffVibration;
            return IsOffVibration;
        }
        #endregion
    }

    #region Gameplay Data
    public enum GameplayStatus { None, Playing, Pause, Win, Lose }

    public enum GameMode
    {
        Tangle = 0,
        DailyChallenge = 1
    }

    public enum StageType
    {
        Tangle = 0,
        StealthNormal = 1,
        StealthElite = 2,
        StealthBoss = 3,
        StealthBonus = 4
    }

    public enum Item
    {
        None = 0,
        Money = 1,
        Gem = 2,
        Axe = 3,
        FreezeTime = 4,
        Hammer = 5
    }

    [System.Serializable]
    public class Reward
    {
        public Item type;
        public int value;

        public Reward()
        {
        }

        public Reward(Item type, int value)
        {
            this.type = type;
            this.value = value;
        }
    }

    [System.Serializable]
    public class Rewards
    {
        public List<Reward> rewards;
    }
    #endregion
}