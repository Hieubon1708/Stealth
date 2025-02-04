using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hunter
{
    public class ButtonScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        Vector3 startScale;

        void Start ()
        {
            startScale = transform.localScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.DOKill();
            transform.DOScale(startScale * 0.95f, 0.1f).SetEase(Ease.Linear);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.DOKill();
            transform.DOScale(startScale, 0.1f).SetEase(Ease.Linear);
        }
    }
}