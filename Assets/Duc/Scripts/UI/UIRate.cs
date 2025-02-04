using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Duc
{
    public class UIRate : UI
    {
        [SerializeField] private List<CanvasGroup> stars;

        private int score = 0;

        public override void Show()
        {
            base.Show();

            score = 5;

            float delay = 0f;
            foreach (var star in stars)
            {
                star.alpha = 0.5f;

                star.transform.DOPunchScale(Vector3.one * 0.1f, 0.25f, 1, 1).SetEase(Ease.OutQuad).SetUpdate(true).OnStart(() =>
                {
                    star.alpha = 1;
                }).SetDelay(delay);

                delay += 0.1f;
            }
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void OnClickStar(int index)
        {
            score = index;

            for (int i = 0; i < stars.Count; i++)
            {
                if (i < index)
                {
                    stars[i].alpha = 1;
                }
                else
                {
                    stars[i].alpha = 0.5f;
                }
            }
        }

        public void OnClickButtonSubmit()
        {
            Manager.instance.RatedGame = true;

            if (score >= 4)
            {
                ACEPlay.Bridge.BridgeController.instance.RateGame();
            }

            UIManager.instance.ShowTextNotice("Thank you for rating us!");
            Hide();
        }

        public void OnClickButtonLater()
        {
            Hide();
        }

        private void OnDestroy()
        {
            foreach (var star in stars)
            {
                star.transform.DOKill();
            }
        }
    }
}