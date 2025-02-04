using UnityEngine;
using DG.Tweening;
using TMPro;

public class TextNotice : MonoBehaviour
{
    public static TextNotice instance;
    [SerializeField] private TextMeshProUGUI txt;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ActiveNotice(string mess)
    {
        txt.DOKill();
        txt.transform.DOKill();
        txt.text = mess;
        txt.DOFade(1, 0f).SetUpdate(true);
        txt.transform.localPosition = Vector3.zero;
        txt.transform.localScale = Vector3.zero;
        txt.gameObject.SetActive(true);
        txt.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
        txt.transform.DOLocalMoveY(200, 1).SetEase(Ease.InOutSine).SetDelay(0.38f).SetUpdate(true);
        txt.DOFade(0, 0.5f).SetEase(Ease.InOutSine).SetDelay(1.2f).OnComplete(() =>
        {
            txt.gameObject.SetActive(false);
        }).SetUpdate(true);
    }
}
