using ACEPlay.Bridge;
using Cinemachine;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Hunter.GameManager;

namespace Hunter
{
    public class UIController : MonoBehaviour
    {
        public static UIController instance;

        public GamePlay gamePlay;
        public Cam virtualCam;
        public Animation glow;
        public Animation camAni;
        public Image layerCover;

        private void Awake()
        {
            instance = this;
        }

        public void Lose()
        {
            gamePlay.Lose();
            layerCover.raycastTarget = true;
            DOVirtual.DelayedCall(2.5f, delegate
            {
                layerCover.raycastTarget = false;
                gamePlay.panelLose.SetActive(true);
            });
        }

        public void Win()
        {
            //EventManager.EmitEvent(EventVariables.StopCountDownShowAdsInGame);
            BridgeController.instance.LogLevelCompleteWithParameter("stealth", GameManager.instance.Level);
            GameManager.instance.Level++;
            gamePlay.Win();
        }

        public void Play()
        {
            //EventManager.EmitEvent(EventVariables.CountDownShowAdsInGame);
            //UIManager.instance.UIHome.Hide();
            GameController.instance.Play();
            virtualCam.CamStartZoom();
            gamePlay.Play();
            if (boss != null)
            {
                GameObject h = boss.transform.Find("Health").gameObject;
                h.SetActive(true);
            }
        }

        public void LoadUI(bool isElevator)
        {
            gamePlay.LoadUI();
            layerCover.raycastTarget = true;
            virtualCam.ResetCam();
            PlayerController.instance.playerTouchMovement.HideTouch();

            StageType stageType = GetStageType();
            if (!isElevator && stageType != StageType.StealthBoss)
            {
                //UIManager.instance.ShowUIHome();
                PlayerController.instance.handTutorial.PlayHand();
            }
            else
            {
                //UIManager.instance.UIHome.Hide();
            }
            gamePlay.frameRemainingEnemy.SetActive(stageType != StageType.StealthBonus);
        }

        public StageType GetStageType()
        {
            return GameManager.instance.levelDataSO.levelTypes[GameManager.instance.Level];
        }

        public void ChangeMap()
        {
            /*StageType stageType = GetStageType();
            if (stageType == StageType.Tangle)
            {
                FadeManager.instance.BackHome(true, true);
            }
            else
            {
                FadeManager.instance.FadeIn(() =>
                {
                    UnityEvent e = new UnityEvent();
                    e.AddListener(() =>
                    {
                        GameController.instance.LoadLevel(GameManager.instance.Level);

                        DOVirtual.DelayedCall(0.5f, () =>
                        {
                            FadeManager.instance.FadeOut();
                        });
                    });

                    if (BridgeController.instance.IsInterReady())
                    {
                        BridgeController.instance.ShowInterstitial("stealth_win", e);
                    }
                    else
                    {
                        e.Invoke();
                        BridgeController.instance.ShowBannerCollapsible();
                    }
                });
            }*/
            GameController.instance.LoadLevel(GameManager.instance.Level);
        }

        public void BossEnd()
        {
            Win();
            GameManager.instance.Win();
            PlayerController.instance.Win();
            GameController.instance.Win();
            DOVirtual.DelayedCall(3.5f, delegate
            {
                layerCover.raycastTarget = false;
                gamePlay.panelWin.SetActive(true);
            });
        }

        Bot boss;

        public IEnumerator BossIntro()
        {
            layerCover.raycastTarget = true;
            PlayerController.instance.handTutorial.StopHand();
            boss = GameController.instance.GetBoss();
            yield return new WaitForSeconds(0.5f);
            CinemachineVirtualCamera cam = boss.GetComponentInChildren<CinemachineVirtualCamera>();
            cam.Priority = 100;
            yield return new WaitForSeconds(2f);
            cam.Priority = 1;
            yield return new WaitForSeconds(1f);
            //UIManager.instance.ShowUIHome();
            PlayerController.instance.handTutorial.PlayHand();
        }

        public void HitEffect()
        {
            ResetHitEffect();
            glow.Play();
        }

        public void HitCancel()
        {
            glow.Stop();
        }

        void ResetHitEffect()
        {
            glow.Stop();
        }
    }
}