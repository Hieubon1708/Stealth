using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Hunter
{
    public class TakeMoney : MonoBehaviour
    {
        int currentTook;
        public TextMeshProUGUI amount;
        public CanvasGroup canvasGroup;

        public void TakeOn(int coin)
        {
            CancelInvoke("TakeOff");
            canvasGroup.DOKill();
            canvasGroup.DOFade(1f, 0.5f);
            currentTook += coin;
            amount.text = "+" + currentTook;
            Invoke("TakeOff", 1.5f);
        }

        public void TakeOff()
        {
            canvasGroup.DOFade(0f, 0.5f).OnComplete(delegate
            {
                currentTook = 0;
            });
        }
    }
}
