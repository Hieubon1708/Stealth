using Duc.PoppyTangle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TigerForge;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Duc
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        private const string pathUISetting = "UI/Panel Setting";
        private const string pathUIShop = "UI/Panel Shop";
        private const string pathUIMission = "UI/Panel Mission";
        private const string pathUILose = "UI/Panel Lose";
        private const string pathUIUseBooster = "UI/Panel Use Booster";
        private const string pathUIBuyBooster = "UI/Panel Buy Booster";
        private const string pathTextNotice = "UI/Text Notice";
        private const string pathUIVipPack = "UI/Panel Vip Pack";
        private const string pathUIRate = "UI/Panel Rate";
        private const string pathUIWinChallenge = "UI/Panel Win Challenge";
        private const string pathCheatMenu = "UI/Panel Cheat";

        [Header("UI Panel")]
        public UIHome UIHome;
        public UIInGame UIInGame;
        public UILevelProgress UILevelProgress;
        [HideInInspector]
        public UISetting UISetting;
        [HideInInspector]
        public UIShop UIShop;
        [HideInInspector]
        public UIMission UIMission;
        [HideInInspector]
        public UILose UILose;
        [HideInInspector]
        public UIUseBooster UIUseBooster;
        [HideInInspector]
        public UIBuyBooster UIBuyBooster;
        [HideInInspector]
        public TextNotice TextNotice;
        [HideInInspector]
        public UIVipPack UIVipPack;
        [HideInInspector]
        public UIRate UIRate;
        [HideInInspector]
        public UIWinChallenge UIWinChallenge;
        [HideInInspector]
        public UICheat UICheat;

        [Header("Canvas")]
        public Canvas canvasUI1;
        [SerializeField] Transform parentUI1;
        [SerializeField] Transform parentUI2;

        public ShopDataSO shopDataSO;

        [SerializeField] private TextMeshProUGUI txtCountDownShowAds;

        private float countDownShowAds;
        private Coroutine coroutineCountDownShowAds;
        private bool isShowingInterInGame = false;

        private void Awake()
        {
            if (instance == null) instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            CheckVipExpired();
            ShowUIHome();

            EventManager.StartListening(EventVariables.CountDownShowAdsInGame, CountDownShowAds);
            EventManager.StartListening(EventVariables.StopCountDownShowAdsInGame, StopCountDownShowAds);
        }

        public void CountDownShowAds()
        {
            if (ACEPlay.Bridge.BridgeController.instance.CanShowInterIngame)
            {
                countDownShowAds = ACEPlay.Bridge.BridgeController.instance.TimeShowInterIngame - 3;

                if (coroutineCountDownShowAds != null)
                {
                    StopCoroutine(coroutineCountDownShowAds);
                }

                coroutineCountDownShowAds = StartCoroutine(IE_CountDownShowAds());
            }
        }

        public void StopCountDownShowAds()
        {
            if (coroutineCountDownShowAds != null)
            {
                StopCoroutine(coroutineCountDownShowAds);
                coroutineCountDownShowAds = null;
            }
            
            if (coroutineShowInter != null)
            {
                StopCoroutine(coroutineShowInter);
                coroutineShowInter = null;
            }

            txtCountDownShowAds.gameObject.SetActive(false);
        }

        private IEnumerator IE_CountDownShowAds()
        {
            while (countDownShowAds > 0)
            {
                countDownShowAds -= Time.unscaledDeltaTime;
                yield return null;
            }

            if (ACEPlay.Bridge.BridgeController.instance.CanShowInterIngame && !isShowingInterInGame)
            {
                isShowingInterInGame = true;

                if (coroutineShowInter != null)
                {
                    StopCoroutine(coroutineShowInter);
                }
                coroutineShowInter = StartCoroutine(IE_ShowInter());
            }
        }

        Coroutine coroutineShowInter;
        IEnumerator IE_ShowInter()
        {
            int chapter = Mathf.Min(Manager.instance.Chapter, Manager.instance.levelDataSO.chapters.Count);
            int stage = Manager.instance.Stage;
            Chapter currentChapter = Manager.instance.levelDataSO.chapters[chapter - 1];

            txtCountDownShowAds.gameObject.SetActive(true);

            for (int i = 3; i > 0; i--)
            {
                txtCountDownShowAds.text = $"Ad in {i}s";
                yield return new WaitForSecondsRealtime(1f);

                if (currentChapter.stages[stage - 1] == StageType.Tangle)
                {
                    yield return new WaitUntil(() => GameplayController.instance.gameplayStatus == GameplayStatus.Playing);
                }
            }

            txtCountDownShowAds.gameObject.SetActive(false);

            UnityEvent e = new UnityEvent();
            e.AddListener(() =>
            {
                isShowingInterInGame = false;

                CountDownShowAds();
            });
            ACEPlay.Bridge.BridgeController.instance.ShowInterstitial("ingame", e, null, true);
        }

        public void CheckVipExpired()
        {
            if (Manager.instance.IsVip)
            {
                string dateExpiredStr = Manager.instance.DateExpiredVip;
                if (!string.IsNullOrEmpty(dateExpiredStr))
                {
                    if (DateTime.TryParseExact(dateExpiredStr, MyTime.instance.format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateExpired))
                    {
                        DateTime currentDate = MyTime.instance.GetCurrentTime(true);
                        TimeSpan diffDate = dateExpired.Subtract(currentDate);
                        if (diffDate.Days <= 0)
                        {
                            Manager.instance.IsVip = false;
                            Manager.instance.DateExpiredVip = null;
                        }
                    }
                    else
                    {
                        Debug.LogError(string.Format("Can't convert DateTime {0}\nAt CheckVipExpired; from UIManager.cs", dateExpiredStr));
                    }
                }
            }
        }

        public void ShowUICheat()
        {
            if (UICheat == null)
            {
                GameObject obj = Resources.Load<GameObject>(pathCheatMenu);
                GameObject go = Instantiate(obj);
                go.transform.SetParent(parentUI2);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;

                UICheat = go.GetComponent<UICheat>();
            }
            UICheat.Show();
        }


        public void ShowUIHome()
        {
            UIHome.Show();
        }

        public void ShowUIIngame()
        {
            UIInGame.Show();
        }

        public void ShowUILevelProgress()
        {
            if (!UILevelProgress.gameObject.activeSelf)
            {
                UILevelProgress.Show();
            }
        }

        public void ShowUISetting()
        {
            if (UISetting == null)
            {
                GameObject obj = Resources.Load<GameObject>(pathUISetting);
                GameObject go = Instantiate(obj);
                go.transform.SetParent(parentUI2);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;

                UISetting = go.GetComponent<UISetting>();
            }

            UISetting.Show();
        }

        public void ShowUIShop(bool home)
        {
            if (UIShop == null)
            {
                GameObject obj = Resources.Load<GameObject>(pathUIShop);
                GameObject go = Instantiate(obj);
                go.transform.SetParent(parentUI1);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;

                UIShop = go.GetComponent<UIShop>();
            }

            UIShop.Show();

            if (home)
            {
                UIShop.transform.SetAsFirstSibling();
            }
            else
            {
                UIShop.ShowCloseButton();
            }
        }

        public void ShowUIVipPack()
        {
            if (UIVipPack == null)
            {
                GameObject obj = Resources.Load<GameObject>(pathUIVipPack);
                GameObject go = Instantiate(obj);
                go.transform.SetParent(parentUI2);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;

                UIVipPack = go.GetComponent<UIVipPack>();
            }

            UIVipPack.Show();
        }

        public void ShowUIMission()
        {
            if (UIMission == null)
            {
                GameObject obj = Resources.Load<GameObject>(pathUIMission);
                GameObject go = Instantiate(obj);
                go.transform.SetParent(parentUI1);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;

                UIMission = go.GetComponent<UIMission>();
            }

            UIMission.Show();

            UIMission.transform.SetAsFirstSibling();
        }

        public void ShowUILose()
        {
            if (UILose == null)
            {
                GameObject obj = Resources.Load<GameObject>(pathUILose);
                GameObject go = Instantiate(obj);
                go.transform.SetParent(parentUI1);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;

                UILose = go.GetComponent<UILose>();
            }

            UILose.Show();
        }

        public void ShowUIUseBooster()
        {
            if (UIUseBooster == null)
            {
                GameObject obj = Resources.Load<GameObject>(pathUIUseBooster);
                GameObject go = Instantiate(obj);
                go.transform.SetParent(parentUI2);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;

                UIUseBooster = go.GetComponent<UIUseBooster>();
            }

            UIUseBooster.Show();
        }

        public void ShowUIBuyBooster()
        {
            if (UIBuyBooster == null)
            {
                GameObject obj = Resources.Load<GameObject>(pathUIBuyBooster);
                GameObject go = Instantiate(obj);
                go.transform.SetParent(parentUI1);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;

                UIBuyBooster = go.GetComponent<UIBuyBooster>();
            }

            UIBuyBooster.Show();
        }

        public void ShowUIWinChallenge()
        {
            if (UIWinChallenge == null)
            {
                GameObject obj = Resources.Load<GameObject>(pathUIWinChallenge);
                GameObject go = Instantiate(obj);
                go.transform.SetParent(parentUI1);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;

                UIWinChallenge = go.GetComponent<UIWinChallenge>();
            }

            UIWinChallenge.Show();
        }

        public void ShowTextNotice(string mess)
        {
            if (TextNotice == null)
            {
                GameObject obj = Resources.Load<GameObject>(pathTextNotice);
                GameObject go = Instantiate(obj);
                go.transform.SetParent(parentUI2);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;

                TextNotice = go.GetComponent<TextNotice>();
            }

            TextNotice.Show();
            TextNotice.ActiveNotice(mess);
        }

        public void ShowUIRate()
        {
            int stageWin = Manager.instance.StageWin;
            if (!Manager.instance.RatedGame && stageWin > 0 && stageWin % 7 == 0 && !Manager.instance.RateShowed)
            {
                if (UIRate == null)
                {
                    GameObject obj = Resources.Load<GameObject>(pathUIRate);
                    GameObject go = Instantiate(obj);
                    go.transform.SetParent(parentUI2);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one;

                    UIRate = go.GetComponent<UIRate>();
                }

                UIRate.Show();

                Manager.instance.RateShowed = true;
            }
        }

        #region Touch and Mouse
        public bool OnPointerDown()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                return true;
            }
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                return true;
            }
        }
#endif
            return false;
        }

        public bool OnPointerUp()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonUp(0))
            {
                return true;
            }
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                return true;
            }
        }
#endif
            return false;
        }

        public bool IsPoiterUI()
        {
#if UNITY_EDITOR
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return true;
            }
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            // Check if finger is over a UI element
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return true;
            }
        }
#endif
            return false;
        }

        public Vector3 PositionTouch()
        {
#if UNITY_EDITOR
            return Input.mousePosition;
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            return Input.GetTouch(0).position;
        }
#endif
            return Vector3.zero;
        }
        #endregion

    }
}