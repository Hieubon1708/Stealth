using DG.Tweening;
using Duc;
using Duc.PoppyTangle;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance;

    [SerializeField] private GameObject background;
    [SerializeField] private Transform shape;
    [SerializeField] private GameObject raycast;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this);
        }
        //else
        //{
        //    Destroy(gameObject);
        //}
    }

    public void Fade(Action onDoneFadeIn = null, Action onDoneFadeOut = null, float delayFadeIn = 0f, float delayFadeOut = 0.5f)
    {
        raycast.SetActive(true);
        background.SetActive(true);
        shape.localScale = Vector3.one * 30f;

        shape.DOScale(0f, 0.5f).SetEase(Ease.OutQuad).SetDelay(delayFadeIn).OnComplete(() =>
        {
            onDoneFadeIn?.Invoke();
            shape.DOScale(30f, 0.5f).SetEase(Ease.InQuad).SetDelay(delayFadeOut).OnComplete(() =>
            {
                onDoneFadeOut?.Invoke();

                background.SetActive(false);
                raycast.SetActive(false);
            }).SetUpdate(true);
        }).SetUpdate(true);
    }

    public void FadeIn(Action onDone = null)
    {
        raycast.SetActive(true);
        background.SetActive(true);
        shape.localScale = Vector3.one * 30f;

        shape.DOScale(0f, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            onDone?.Invoke();
        }).SetUpdate(true);
    }

    public void FadeOut(Action onDone = null)
    {
        raycast.SetActive(true);
        background.SetActive(true);
        shape.localScale = Vector3.zero;

        shape.DOScale(30f, 0.5f).SetEase(Ease.InQuad).SetDelay(0.5f).OnComplete(() =>
        {
            background.SetActive(false);
            raycast.SetActive(false);
            onDone?.Invoke();
        }).SetUpdate(true);
    }

    public void LoadScene(int id, Action onDoneFadeIn = null, bool isWinTangle = false)
    {
        FadeIn(() =>
        {
            UnityEvent e = new UnityEvent();
            e.AddListener(() =>
            {
                onDoneFadeIn?.Invoke();
                StartCoroutine(LoadAsyncGame(id));
            });

            if (isWinTangle)
            {
                if (ACEPlay.Bridge.BridgeController.instance.IsInterReady())
                {
                    ACEPlay.Bridge.BridgeController.instance.ShowInterstitial("tangle_win", e);
                }
                else
                {
                    e.Invoke();
                    ACEPlay.Bridge.BridgeController.instance.ShowBannerCollapsible();
                }
            }
            else
            {
                e.Invoke();
            }
            
        });
    }

    public void LoadSceneOnStart(int id, Action onDoneFadeIn = null)
    {
        raycast.SetActive(true);
        background.SetActive(true);
        shape.localScale = Vector3.zero;
        onDoneFadeIn?.Invoke();
        StartCoroutine(LoadAsyncGame(id));
    }

    IEnumerator LoadAsyncGame(int scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            float progress = operation.progress;
            if (progress == 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        FadeOut();
    }

    public void BackHome(bool isWin, bool showInter = false, Action onDoneFadeIn = null)
    {
        FadeIn(() =>
        {
            string placement = isWin ? "stealth_win" : "stealth_lose";

            UnityEvent e = new UnityEvent();
            e.AddListener(() =>
            {
                onDoneFadeIn?.Invoke();
                StartCoroutine(UnloadAsyncGame(2));
            });

            if (showInter)
            {
                if (ACEPlay.Bridge.BridgeController.instance.IsInterReady())
                {
                    ACEPlay.Bridge.BridgeController.instance.ShowInterstitial(placement, e);
                }
                else
                {
                    e.Invoke();
                    ACEPlay.Bridge.BridgeController.instance.ShowBannerCollapsible();
                }
            }
            else
            {
                e.Invoke();
            }
        });
    }

    IEnumerator UnloadAsyncGame(int scene)
    {
        AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
        while (!operation.isDone)
        {
            yield return null;
        }

        UIManager.instance.ShowUIHome();
        GameplayController.instance.SetUp();

        FadeOut();

        AudioController.instance.PlayMenuMusic();
    }

    public void UnloadSceneStealth(Action onDoneUnload = null)
    {
        FadeIn(() =>
        {
            StartCoroutine(UnloadAsyncGame(2, onDoneUnload));
        });
    }

    IEnumerator UnloadAsyncGame(int scene, Action onDone = null)
    {
        AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
        while (!operation.isDone)
        {
            yield return null;
        }

        onDone?.Invoke();

        FadeOut();

        AudioController.instance.PlayMenuMusic();
    }
}
