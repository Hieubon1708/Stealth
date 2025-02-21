using ACEPlay.Bridge;
using Cinemachine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using static Hunter.GameManager;

namespace Hunter
{
    public class GameController : MonoBehaviour
    {
        public static GameController instance;

        public Transform poolEnemy;
        public Transform poolPoppy;
        public Transform poolWeapon;

        public Camera cam;
        public Camera camUI;
        public WeaponEquip weaponEquip;

        public List<Bot> bots;
        public List<Bot> botsReserve;
        public PathInfo[] pathInfos;

        public List<Player> poppies;
        public GameObject[] prePoppies;
        public List<Character> tempPoppies;
        public List<WeaponType> tempWeaponPoppies;

        public CinemachineVirtualCamera camFollow;
        public GameObject map;
        public bool isAlert;
        public AlertType alertType;
        public AlertCamera[] alertCameras;
        public Laser[] lasers;
        public Turrel[] turrels;
        public Barrel[] barrels;
        public Door[] doors;
        public Alarm[] alarms;
        public ObjectBroken[] objectBrokens;
        public Transform container;
        public ParticleSystem fxBum;
        public GameObject[] preBotHealths;
        public Dictionary<GameObject, WeaponType> tempWeaponOnGround = new Dictionary<GameObject, WeaponType>();

        public void Awake()
        {
            instance = this;
            camUI.enabled = false;
            camUI.enabled = true;
        }

        public void Start()
        {
            Init();
        }

        public void Init()
        {
            LoadLevel(GameManager.instance.Level);
        }

        public enum BotType
        {
            Normal, Boss
        }

        public enum PathType
        {
            Repeat, Circle
        }

        public enum WeaponType
        {
            None, Knife, Gun
        }

        public enum AlertType
        {
            Camera, Laser
        }

        public enum PoppyType
        {
            Poppy, Bobby, Bubba, Catnap, Craftycorn, Hoppy, Kickin, Pickypiggy
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                // do something
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                // do something
            }
        }

        public bool IsKilling()
        {
            for (int i = 0; i < poppies.Count; i++)
            {
                if (poppies[i].isKilling) return true;
            }
            return false;
        }

        public bool IsAttack()
        {
            for (int i = 0; i < poppies.Count; i++)
            {
                if (poppies[i].isAttack) return true;
            }
            return false;
        }

        public void SetAngularSpeed(float angularSpeed)
        {
            for (int i = 0; i < poppies.Count; i++)
            {
                poppies[i].navMeshAgent.angularSpeed = angularSpeed;
            }
        }

        public void LoadLevel(int level)
        {
            BridgeController.instance.LogLevelStartWithParameter("stealk", GameManager.instance.Level);
            AudioController.instance.ResetAudio();
            BridgeController.instance.Debug_Log(level.ToString());
            poppies.Clear();
            bots.Clear();
            PlayerController.instance.ResetFxDollars();
            for (int i = 0; i < poolEnemy.childCount; i++)
            {
                Destroy(poolEnemy.GetChild(i).gameObject);
            }
            for (int i = 0; i < poolPoppy.childCount; i++)
            {
                Destroy(poolPoppy.GetChild(i).gameObject);
            }

            if (map != null) Destroy(map);
            map = Instantiate(Resources.Load<GameObject>(level.ToString()), container);
            PlayerController.instance.ResetGame();
            LoadPoppy();
            for (int i = 0; i < poppies.Count; i++)
            {
                poppies[i].ResetPlayer();
            }
            pathInfos = map.GetComponentsInChildren<PathInfo>();
            alertCameras = map.GetComponentsInChildren<AlertCamera>();
            lasers = map.GetComponentsInChildren<Laser>();
            turrels = map.GetComponentsInChildren<Turrel>();
            barrels = map.GetComponentsInChildren<Barrel>();
            alarms = map.GetComponentsInChildren<Alarm>();
            objectBrokens = map.GetComponentsInChildren<ObjectBroken>();
            botsReserve = new List<Bot>(bots);
            ResetBots();
            AIManager.Instance.Init();
            StartDoor startDoor = map.GetComponentInChildren<StartDoor>();
            UIController.instance.LoadUI(startDoor != null);
            if (startDoor != null) StartCoroutine(startDoor.ElevatorMoveUp(poppies));
            else
            {
                if (IsBoss()) StartCoroutine(UIController.instance.BossIntro());
            }
        }

        public void AddWeapon(WeaponType weaponType)
        {
            int min = 100;
            for (int i = 0; i < poppies.Count; i++)
            {
                if(poppies[i].weapon == null)
                {
                    min = 0; break;
                }
                if ((int)poppies[i].weapon.weaponType < min)
                {
                    min = (int)poppies[i].weapon.weaponType;
                }
            }
            for (int i = 0; i < poppies.Count; i++)
            {
                if (poppies[i].weapon == null)
                {
                    LoadWeaponPoppy(weaponType, poppies[i]);
                    return;
                }
            }
            int indexRandom = Random.Range(0, poppies.Count);
            LoadWeaponPoppy(weaponType, poppies[indexRandom]);
        }

        public void Play()
        {
            StartBots();
            for (int i = 0; i < turrels.Length; i++)
            {
                turrels[i].Play();
            }
            for (int i = 0; i < alertCameras.Length; i++)
            {
                alertCameras[i].Play();
            }
        }

        public void StartBots()
        {
            StartProbes();
        }

        void LoadPoppy()
        {
            tempPoppies = new List<Character>(GameManager.instance.RescuedCharacter);
            tempWeaponPoppies = new List<WeaponType>(GameManager.instance.WeaponCharacter);
            for (int i = 0; i < tempPoppies.Count; i++)
            {
                AddPoppy(tempPoppies[i], tempWeaponPoppies[i]);
            }
            if (tempPoppies.Count == 0)
            {
                AddPoppy(Character.Male, WeaponType.None);
            }
        }

        public bool IsBoss()
        {
            for (int i = 0; i < pathInfos.Length; i++)
            {
                if (pathInfos[i].botType == BotType.Boss) return true;
            }
            return false;
        }

        public Bot GetBoss()
        {
            for (int i = 0; i < pathInfos.Length; i++)
            {
                if (pathInfos[i].botType == BotType.Boss) return bots[i];
            }
            return null;
        }

        public void RemovePoppy(Player poppy)
        {
            int indexOf = poppies.IndexOf(poppy);
            if (indexOf == -1)
            {
                Debug.LogError("!!! " + poppy.name);
                Debug.LogError("Poppies Count " + poppies.Count);
            }
            poppies.Remove(poppy);
            if (poppies.Count == 0)
            {
                PlayerController.instance.Lose();
                UIController.instance.Lose();
                EndDoor endDoor = map.GetComponentInChildren<EndDoor>();
                if (endDoor != null) endDoor.StopDoor();
                PlayerController.instance.playerTouchMovement.navMeshAgent.isStopped = true;
            }
        }

        void LoadWeaponPoppy(WeaponType weaponType, Player player)
        {
            player.LoadWeapon(weaponType);
        }

        public Player GetPoppy(GameObject poppy)
        {
            for (int i = 0; i < poppies.Count; i++)
            {
                if (poppies[i].gameObject == poppy)
                {
                    return poppies[i];
                }
            }
            return null;
        }

        public Laser GetLaser(GameObject laser)
        {
            for (int i = 0; i < lasers.Length; i++)
            {
                if (lasers[i].gameObject == laser)
                {
                    return lasers[i];
                }
            }
            return null;
        }

        public AlertCamera GetCamera(GameObject camera)
        {
            for (int i = 0; i < alertCameras.Length; i++)
            {
                if (alertCameras[i].gameObject == camera)
                {
                    return alertCameras[i];
                }
            }
            return null;
        }

        public Player AddPoppy(GameManager.Character poppy, WeaponType weaponType)
        {
            GameObject p = Instantiate(prePoppies[(int)poppy], poolPoppy);
            Player sc = p.GetComponent<Player>();
            poppies.Add(sc);
            LoadWeaponPoppy(weaponType, sc);
            return sc;
        }

        void RemovePoppies()
        {
            for (int i = 0; i < poppies.Count; i++)
            {
                if (!poppies[i].gameObject.activeSelf)
                {
                    Destroy(poppies[i].gameObject);
                }
            }
        }

        public void SwitchWeapon(Player player)
        {
            if (poppies.Count == 1 && poppies[0].weapon == null)
            {
                Vector3 pos = poppies[0].transform.position;
                fxBum.transform.position = new Vector3(pos.x, pos.y + 1, pos.z);
                player.weapon.gameObject.SetActive(false);
                fxBum.Play();
                LoadWeaponPoppy(player.weapon.weaponType, poppies[0]);
            }
        }

        public void ActiveNavMesh(bool isActive)
        {
            for (int i = 0; i < poppies.Count; i++)
            {
                poppies[i].navMeshAgent.enabled = isActive;
            }
        }

        public void DoMoveY(float y, float time)
        {
            for (int i = 0; i < poppies.Count; i++)
            {
                poppies[i].transform.DOMoveY(y, time);
            }
        }

        public Bot GetBot(GameObject bot)
        {
            for (int i = 0; i < bots.Count; i++)
            {
                if (bots[i].gameObject == bot)
                {
                    return bots[i];
                }
            }
            return null;
        }

        public void RemoveBot(GameObject bot)
        {
            for (int i = 0; i < bots.Count; i++)
            {
                if (bots[i].gameObject == bot)
                {
                    bots.RemoveAt(i);
                }
            }
        }

        public void ResetBots()
        {
            for (int i = 0; i < bots.Count; i++)
            {
                bots[i].ResetBot();
            }
        }

        public void SetBot(BotType botType, PathInfo pathInfo)
        {
            bots.Add(Instantiate(pathInfo.prefab, poolEnemy).GetComponent<Bot>());
            bots[bots.Count - 1].Init(pathInfo);
        }

        public void StartProbes()
        {
            for (int i = 0; i < bots.Count; i++)
            {
                bots[i].StartProbe(1);
            }
        }

        public Barrel GetBarrel(GameObject barrel)
        {
            for (int i = 0; i < barrels.Length; i++)
            {
                if (barrels[i].gameObject == barrel) return barrels[i];
            }
            return null;
        }

        public Bot GetBoneNBot(List<GameObject> bones, GameObject bone)
        {
            Bot bot = null;
            for (int i = 0; i < botsReserve.Count; i++)
            {
                for (int j = 0; j < botsReserve[i].rbs.Length; j++)
                {
                    if (botsReserve[i].rbs[j].gameObject == bone)
                    {
                        bot = botsReserve[i];
                        break;
                    }
                }
                if (bot != null) break;
            }
            if (bot != null)
            {
                for (int i = 0; i < bot.rbs.Length; i++)
                {
                    bones.Add(bot.rbs[i].gameObject);
                }
            }
            return bot;
        }

        public void Alert(AlertType alertType, GameObject target)
        {
            CancelInvoke(nameof(AlertOff));
            this.alertType = alertType;
            isAlert = true;
            AudioController.instance.PlayAlert();
            for (int i = 0; i < bots.Count; i++)
            {
                if (bots[i] is SentryBot && bots[i].col.enabled)
                {
                    SentryBot sentryBot = (SentryBot)bots[i];
                    if (sentryBot.questionRotate.transform.localScale == Vector3.zero)
                    {
                        sentryBot.StopProbe();
                        sentryBot.StopLostTrack();
                        sentryBot.StartLostTrack(target);
                    }
                }
            }
            for (int i = 0; i < alarms.Length; i++)
            {
                alarms[i].Alert();
            }
            Invoke(nameof(AlertOff), 7);
        }

        public void Win()
        {
            SetAngularSpeed(500);
        }

        void AlertOff()
        {
            AudioController.instance.StopAlert();
            isAlert = false;
            for (int i = 0; i < alarms.Length; i++)
            {
                alarms[i].StopAlert();
            }
        }
    }
}
