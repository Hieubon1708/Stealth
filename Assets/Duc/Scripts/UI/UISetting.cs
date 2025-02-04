using DG.Tweening;
using Duc.PoppyTangle;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duc
{
    public class UISetting : UI
    {
        [Header("Music")]
        [SerializeField] private RectTransform toggleMusic;
        [SerializeField] private Image bgSetMusic;

        [Header("Sound")]
        [SerializeField] private RectTransform toggleSound;
        [SerializeField] private Image bgSetSound;

        [Header("Vibrate")]
        [SerializeField] private RectTransform toggleVibrate;
        [SerializeField] private Image bgSetVibrate;

        [Header("Sprite")]
        [SerializeField] private Sprite sprBgOn;
        [SerializeField] private Sprite sprBgOff;

        [Header("Group")]
        [SerializeField] private GameObject groupSocial;
        [SerializeField] private GameObject groupInGame;

        public override void Start()
        {
            base.Start();
        }

        public override void Show()
        {
            base.Show();

            if (Manager.instance.IsMuteMusic)
            {
                toggleMusic.anchoredPosition = new Vector2(-63, toggleMusic.anchoredPosition.y);
                bgSetMusic.sprite = sprBgOff;
            }
            else
            {
                toggleMusic.anchoredPosition = new Vector2(63, toggleMusic.anchoredPosition.y);
                bgSetMusic.sprite = sprBgOn;
            }

            if (Manager.instance.IsMuteSound)
            {
                toggleSound.anchoredPosition = new Vector2(-63, toggleSound.anchoredPosition.y);
                bgSetSound.sprite = sprBgOff;
            }
            else
            {
                toggleSound.anchoredPosition = new Vector2(63, toggleSound.anchoredPosition.y);
                bgSetSound.sprite = sprBgOn;
            }

            if (Manager.instance.IsOffVibration)
            {
                toggleVibrate.anchoredPosition = new Vector2(-63, toggleVibrate.anchoredPosition.y);
                bgSetVibrate.sprite = sprBgOff;
            }
            else
            {
                toggleVibrate.anchoredPosition = new Vector2(63, toggleVibrate.anchoredPosition.y);
                bgSetVibrate.sprite = sprBgOn;
            }

            if (GameplayController.instance.gameplayStatus == GameplayStatus.Playing)
            {
                GameplayController.instance.gameplayStatus = GameplayStatus.Pause;

                groupSocial.SetActive(false);
                groupInGame.SetActive(true);
            }
            else
            {
                groupSocial.SetActive(true);
                groupInGame.SetActive(false);
            }

            ACEPlay.Native.NativeAds.instance.DisplayNativeAds(true);
            ACEPlay.Bridge.BridgeController.instance.ShowBannerCollapsible();
        }

        public override void Hide()
        {
            if (GameplayController.instance.gameplayStatus == GameplayStatus.Pause)
            {
                GameplayController.instance.gameplayStatus = GameplayStatus.Playing;
            }

            ACEPlay.Native.NativeAds.instance.DisplayNativeAds(false);
            ACEPlay.Bridge.BridgeController.instance.HideBannerCollapsible();

            base.Hide();
        }

        public void SetMusic()
        {
            if (Manager.instance.SetMusic())
            {
                toggleMusic.DOAnchorPosX(-63, 0.15f).SetEase(Ease.OutBack).SetUpdate(true);
                bgSetMusic.sprite = sprBgOff;
            }
            else
            {
                toggleMusic.DOAnchorPosX(63, 0.15f).SetEase(Ease.OutBack).SetUpdate(true);
                bgSetMusic.sprite = sprBgOn;
            }
        }

        public void SetSound()
        {
            if (Manager.instance.SetSound())
            {
                toggleSound.DOAnchorPosX(-63, 0.15f).SetEase(Ease.OutBack).SetUpdate(true);
                bgSetSound.sprite = sprBgOff;
            }
            else
            {
                toggleSound.DOAnchorPosX(63, 0.15f).SetEase(Ease.OutBack).SetUpdate(true);
                bgSetSound.sprite = sprBgOn;
            }
        }

        public void SetVibrate()
        {
            if (Manager.instance.SetVibration())
            {
                toggleVibrate.DOAnchorPosX(-63, 0.15f).SetEase(Ease.OutBack).SetUpdate(true);
                bgSetVibrate.sprite = sprBgOff;
            }
            else
            {
                toggleVibrate.DOAnchorPosX(63, 0.15f).SetEase(Ease.OutBack).SetUpdate(true);
                bgSetVibrate.sprite = sprBgOn;
            }
        }

        public void OnClickButtonClose()
        {
            Hide();
        }

        public void OnClickButtonHome()
        {
            string placement;
            if (GameplayController.instance.gameMode == GameMode.Tangle)
            {
                placement = "tangle_backhome";
            }
            else
            {
                placement = "challenge_backhome";
            }

            UnityEvent e = new UnityEvent();
            e.AddListener(() =>
            {
                if (GameplayController.instance.gameMode == GameMode.Tangle)
                {
                    FadeManager.instance.Fade(() =>
                    {
                        GameplayController.instance.QuitGame();
                        UIManager.instance.UIInGame.Hide();
                        Hide();
                        UIManager.instance.ShowUIHome();
                        GameplayController.instance.SetUp();

                        ACEPlay.Bridge.BridgeController.instance.LogLevelFailWithParameter("tangle", Manager.instance.LevelPoppyTangle);
                    });
                }
                else
                {
                    int chapter = Mathf.Min(Manager.instance.Chapter, Manager.instance.levelDataSO.chapters.Count);
                    int stage = Manager.instance.Stage;

                    Chapter currentChapter = Manager.instance.levelDataSO.chapters[chapter - 1];

                    if (currentChapter.stages[stage - 1] == StageType.Tangle)
                    {
                        FadeManager.instance.Fade(() =>
                        {
                            GameplayController.instance.QuitGame();
                            UIManager.instance.UIInGame.Hide();
                            Hide();
                            UIManager.instance.ShowUIHome();
                            GameplayController.instance.SetUp();
                        });
                    }
                    else
                    {
                        FadeManager.instance.LoadScene(2, () =>
                        {
                            GameplayController.instance.QuitGame();
                            UIManager.instance.UIInGame.Hide();
                            Hide();
                            //UIManager.instance.ShowUIHome();
                            AudioController.instance.StopMenuMusic();
                        });
                    }

                    ACEPlay.Bridge.BridgeController.instance.LogLevelFailWithParameter("challenge", MissionManager.instance.LevelTwistedChallenge);
                }
            });
            ACEPlay.Bridge.BridgeController.instance.ShowInterstitial(placement, e);
        }

        public void OnClickButtonReplay()
        {
            string placement;
            if (GameplayController.instance.gameMode == GameMode.Tangle)
            {
                placement = "tangle_replay";
            }
            else
            {
                placement = "challenge_replay";
            }

            UnityEvent e = new UnityEvent();
            e.AddListener(() =>
            {
                FadeManager.instance.Fade(() =>
                {
                    //GameplayController.instance.QuitGame();
                    //UIManager.instance.UIInGame.Hide();
                    Hide();
                    //UIManager.instance.ShowUIHome();
                    GameplayController.instance.Init();

                    if (GameplayController.instance.gameMode == GameMode.Tangle)
                    {
                        ACEPlay.Bridge.BridgeController.instance.LogLevelFailWithParameter("tangle", Manager.instance.LevelPoppyTangle);
                    }
                    else
                    {
                        ACEPlay.Bridge.BridgeController.instance.LogLevelFailWithParameter("challenge", MissionManager.instance.LevelTwistedChallenge);
                    }
                });
            });
            ACEPlay.Bridge.BridgeController.instance.ShowInterstitial(placement, e);
        }

        public void OnClickButtonYoutube()
        {
            UnityEvent e = new UnityEvent();
            e.AddListener(() =>
            {
                UIManager.instance.ShowTextNotice("Thanks for supporting us!");
            });
            ACEPlay.Bridge.BridgeController.instance.SubcribeYoutube(e);
        }
        
        public void OnClickButtonTiktok()
        {
            UnityEvent e = new UnityEvent();
            e.AddListener(() =>
            {
                UIManager.instance.ShowTextNotice("Thanks for supporting us!");
            });
            ACEPlay.Bridge.BridgeController.instance.FollowTikTok(e);
        }

        public void OnClickButtonFacebook()
        {
            UnityEvent e = new UnityEvent();
            e.AddListener(() =>
            {
                UIManager.instance.ShowTextNotice("Thanks for supporting us!");
            });
            ACEPlay.Bridge.BridgeController.instance.FollowFacebook(e);
        }
    }
}
