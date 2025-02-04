using DG.Tweening;
using Duc.PoppyTangle;
using System.Collections;
using System.Collections.Generic;
using TigerForge;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duc
{
    public class UIInGame : UI
    {
        [SerializeField] private TextMeshProUGUI txtTimeCountDown;

        [SerializeField] private GameObject UIParent;

        [Header("Freeze Panel")]
        [SerializeField] private CanvasGroup groupFreeze;
        [SerializeField] private ParticleSystem vfxFreeze;
        [SerializeField] private Slider progressCountDownFreezeTime;

        [Header("Button Axe")]
        [SerializeField] private GameObject btnAxe;
        [SerializeField] private TextMeshProUGUI txtCountAxe;
        [SerializeField] private GameObject iconAddAxe;

        [Header("Button Freeze")]
        [SerializeField] private GameObject btnFreeze;
        [SerializeField] private TextMeshProUGUI txtCountFreeze;
        [SerializeField] private GameObject iconAddFreeze;

        [Header("Button Hammer")]
        [SerializeField] private GameObject btnHammer;
        [SerializeField] private TextMeshProUGUI txtCountHammer;
        [SerializeField] private GameObject iconAddHammer;

        [Header("Tutorial")]
        public Transform hand;

        private void Awake()
        {
            EventManager.StartListening(EventVariables.CountDown, CountDown);
            EventManager.StartListening(EventVariables.StopCountDown, StopCountDown);
            EventManager.StartListening(EventVariables.StartCountDown, StartCountDown);
            EventManager.StartListening(EventVariables.EndGame, EndGame);
            EventManager.StartListening(EventVariables.Freeze, Freeze);
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Show()
        {
            base.Show();

            hand.gameObject.SetActive(false);
            canCountDown = false;

            DisplayButtonAxe();
            DisplayButtonFreeze();
            DisplayButtonHammer();
        }

        public override void Hide()
        {
            base.Hide();
            gameObject.SetActive(false);
        }

        public void SetActiveUI(bool active)
        {
            UIParent.SetActive(active);
        }

        private float totalTimeFreeze = 5f;
        private bool isFreeze = false;
        public void Freeze()
        {
            isFreeze = true;

            groupFreeze.gameObject.SetActive(true);
            groupFreeze.alpha = 1;

            vfxFreeze.Play();

            countDownFreeze = totalTimeFreeze;
            progressCountDownFreezeTime.value = 1f;

            coroutineFreeze = StartCoroutine(IE_Freeze());

            AudioController.instance.PlaySoundBoosterFreeze();
        }

        Coroutine coroutineFreeze;
        float countDownFreeze;
        IEnumerator IE_Freeze()
        {
            while (countDownFreeze > 0)
            {
                if (GameplayController.instance.gameplayStatus == GameplayStatus.Playing)
                    countDownFreeze = Mathf.Max(0, countDownFreeze - Time.deltaTime);

                progressCountDownFreezeTime.value = countDownFreeze / totalTimeFreeze;

                yield return null;
            }

            isFreeze = false;
            vfxFreeze.Stop();

            groupFreeze.DOFade(0f, 0.5f).OnComplete(() =>
            {
                groupFreeze.gameObject.SetActive(false);
            });
        }

        Coroutine coroutineCountDown;
        float timeCountDown;
        bool canCountDown = false;

        private void CountDown()
        {
            timeCountDown = EventManager.GetFloat(EventVariables.CountDown);

            if (coroutineCountDown != null)
            {
                StopCoroutine(coroutineCountDown);
            }
            coroutineCountDown = StartCoroutine(CoroutineCountDown());
        }

        void StopCountDown()
        {
            if (coroutineCountDown != null)
            {
                StopCoroutine(coroutineCountDown);
            }

            canCountDown = false;
        }
        
        void StartCountDown()
        {
            canCountDown = true;
        }

        IEnumerator CoroutineCountDown()
        {
            txtTimeCountDown.text = timeCountDown.ToString();
            while (timeCountDown > 0)
            {
                if (GameplayController.instance.gameplayStatus == GameplayStatus.Playing && canCountDown && !isFreeze)
                    timeCountDown = Mathf.Max(0, timeCountDown - Time.deltaTime);

                float minutes = Mathf.FloorToInt(timeCountDown / 60);
                float seconds = Mathf.FloorToInt(timeCountDown % 60);

                txtTimeCountDown.text = string.Format("{0:0}:{1:00}", minutes, seconds);

                yield return null;
            }
            EventManager.EmitEventData(EventVariables.EndGame, false);
        }

        private void EndGame()
        {
            EventManager.EmitEvent(EventVariables.StopCountDownShowAdsInGame);

            bool isWin = EventManager.GetBool(EventVariables.EndGame);
            if (isWin)
            {
                StopCountDown();

                if (coroutineFreeze != null)
                {
                    StopCoroutine(coroutineFreeze);
                    coroutineFreeze = null;

                    isFreeze = false;
                    groupFreeze.gameObject.SetActive(false);
                }

                DOVirtual.DelayedCall(3f, () =>
                {
                    int chapter = Mathf.Min(Manager.instance.Chapter, Manager.instance.levelDataSO.chapters.Count);
                    int stage = Manager.instance.Stage;

                    Chapter currentChapter = Manager.instance.levelDataSO.chapters[chapter - 1];

                    if (currentChapter.stages[stage - 1] == StageType.Tangle)
                    {
                        FadeManager.instance.FadeIn(() =>
                        {
                            UnityEvent e = new UnityEvent();
                            e.AddListener(() =>
                            {
                                GameplayController.instance.QuitGame();
                                Hide();
                                UIManager.instance.ShowUIHome();
                                GameplayController.instance.SetUp();

                                FadeManager.instance.FadeOut();
                            });

                            if (ACEPlay.Bridge.BridgeController.instance.IsInterReady())
                            {
                                ACEPlay.Bridge.BridgeController.instance.ShowInterstitial("tangle_win", e);
                            }
                            else
                            {
                                e.Invoke();

                                ACEPlay.Bridge.BridgeController.instance.ShowBannerCollapsible();
                            }
                        });
                    }
                    else
                    {
                        FadeManager.instance.LoadScene(2, () =>
                        {
                            GameplayController.instance.QuitGame();
                            Hide();
                            //UIManager.instance.ShowUIHome();
                            AudioController.instance.StopMenuMusic();
                        }, true);
                    }
                });
            }
            else
            {
                GameplayController.instance.gameplayStatus = GameplayStatus.Lose;
                UIManager.instance.ShowUILose();
            }
        }

        public void DisplayButtonAxe()
        {
            btnAxe.SetActive(true);

            iconAddAxe.SetActive(false);
            txtCountAxe.gameObject.SetActive(false);

            int totalAxe = Manager.instance.Axe;
            if (totalAxe > 0)
            {
                txtCountAxe.gameObject.SetActive(true);
                txtCountAxe.text = totalAxe.ToString();
            }
            else
            {
                iconAddAxe.SetActive(true);
            }
        }

        public void DisplayButtonFreeze()
        {
            btnFreeze.SetActive(true);

            iconAddFreeze.SetActive(false);
            txtCountFreeze.gameObject.SetActive(false);

            int totalFreeze = Manager.instance.FreezeTime;
            if (totalFreeze > 0)
            {
                txtCountFreeze.gameObject.SetActive(true);
                txtCountFreeze.text = totalFreeze.ToString();
            }
            else
            {
                iconAddFreeze.SetActive(true);
            }
        }

        public void DisplayButtonHammer()
        {
            btnHammer.SetActive(true);

            iconAddHammer.SetActive(false);
            txtCountHammer.gameObject.SetActive(false);

            int totalHammer = Manager.instance.Hammer;
            if (totalHammer > 0)
            {
                txtCountHammer.gameObject.SetActive(true);
                txtCountHammer.text = totalHammer.ToString();
            }
            else
            {
                iconAddHammer.SetActive(true);
            }
        }

        public void OnClickButtonAxe()
        {
            if (GameplayController.instance.gameplayStatus != GameplayStatus.Playing) return;

            int totalAxe = Manager.instance.Axe;
            if (totalAxe > 0)
            {
                EventManager.SetData(EventVariables.UseBooster, Item.Axe);
                UIManager.instance.ShowUIUseBooster();
                btnAxe.SetActive(false);
            }
            else
            {
                EventManager.SetData(EventVariables.BuyBooster, Item.Axe);
                UIManager.instance.ShowUIBuyBooster();
            }
        }

        public void OnClickButtonFreezeTime()
        {
            if (GameplayController.instance.gameplayStatus != GameplayStatus.Playing) return;
            if (isFreeze) return;

            int totalFreezeTime = Manager.instance.FreezeTime;
            if (totalFreezeTime > 0)
            {
                EventManager.SetData(EventVariables.UseBooster, Item.FreezeTime);
                UIManager.instance.ShowUIUseBooster();
                btnFreeze.SetActive(false);
            }
            else
            {
                EventManager.SetData(EventVariables.BuyBooster, Item.FreezeTime);
                UIManager.instance.ShowUIBuyBooster();
            }
        }

        public void OnClickButtonHammer()
        {
            if (GameplayController.instance.gameplayStatus != GameplayStatus.Playing) return;

            int totalHammer = Manager.instance.Hammer;
            if (totalHammer > 0)
            {
                EventManager.SetData(EventVariables.UseBooster, Item.Hammer);
                UIManager.instance.ShowUIUseBooster();
                btnHammer.SetActive(false);
            }
            else
            {
                EventManager.SetData(EventVariables.BuyBooster, Item.Hammer);
                UIManager.instance.ShowUIBuyBooster();
            }
        }

        public void OnClickButtonSetting()
        {
            UIManager.instance.ShowUISetting();
        }

        #region Cheat
        float startTimeCheat;
        int tapCheat;

        public void PressedCheatBtn()
        {
            float diffTime = Time.time - startTimeCheat;
            if (diffTime > 1)
            {
                tapCheat = 1;
                startTimeCheat = Time.time;
            }
            else
            {
                tapCheat++;
                if (tapCheat == 5)
                {
                    // Show Ui Cheat
                    UIManager.instance.ShowUICheat();
                }
            }

            startTimeCheat = Time.time;
        }
        #endregion

    }
}