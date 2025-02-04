using UnityEngine;
using DG.Tweening;
using TMPro;

namespace Duc
{
    public class TextNotice : UI
    {
        [SerializeField] private TextMeshProUGUI txt;

        public override void Start()
        {
            base.Start();
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
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
                Hide();
            }).SetUpdate(true);
        }
    }
}