using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Hunter
{
    public class PlayerTouchMovement : MonoBehaviour
    {
        [SerializeField]
        private Vector2 JoystickSize = new Vector2(300, 300);
        [SerializeField]
        private FloatingJoystick Joystick;
        [SerializeField]

        public Vector2 MovementAmount;

        public RectTransform canvas;
        public NavMeshAgent navMeshAgent;
        public CanvasGroup canvasGroup;

        public Vector2 GetMovemntAmount()
        {
            return MovementAmount;
        }

        public void HandleFingerMove()
        {
            if (GameController.instance.poppies.Count == 0 || UIController.instance.layerCover.raycastTarget) return;
            Vector2 knobPosition;
            float maxMovement = JoystickSize.x / 2f;
            Vector2 clickPosition = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, GameController.instance.camUI, out clickPosition);
            Vector2 touchPos = new Vector2(clickPosition.x, clickPosition.y + canvas.sizeDelta.y / 2);
            if (Vector2.Distance(
                touchPos,
                    Joystick.RectTransform.anchoredPosition
                ) > maxMovement)
            {
                knobPosition = (
                    touchPos - Joystick.RectTransform.anchoredPosition
                    ).normalized
                    * maxMovement;
            }
            else
            {
                knobPosition = touchPos - Joystick.RectTransform.anchoredPosition;
            }
            Joystick.Knob.anchoredPosition = knobPosition;
            MovementAmount = knobPosition / maxMovement;
        }

        public void ShowTouch()
        {
            canvasGroup.DOFade(1f, 0.5f);
        }

        public void HideTouch()
        {
            canvasGroup.DOKill();
            canvasGroup.alpha = 0;
        }

        public void HandleLoseFinger()
        {
            Joystick.Knob.anchoredPosition = Vector2.zero;
            MovementAmount = Vector2.zero;
            Joystick.RectTransform.anchoredPosition = new Vector2(0, 350);
        }

        public void HandleFingerDown()
        {
            if (PlayerController.instance.handTutorial.canvasGroup.alpha != 0)
            {
                UnityEvent e = new UnityEvent();
                e.AddListener(() =>
                {
                    ShowTouch();
                    UIController.instance.Play();
                });

                if (ACEPlay.Bridge.BridgeController.instance.IsShowAdsPlay)
                {
                    ACEPlay.Bridge.BridgeController.instance.ShowInterstitial("stealth_play", e);
                }
                else
                {
                    e.Invoke();
                }
            }
            Vector2 clickPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, GameController.instance.camUI, out clickPosition);
            MovementAmount = Vector2.zero;
            Joystick.RectTransform.sizeDelta = JoystickSize;
            Joystick.RectTransform.anchoredPosition = ClampStartPosition(new Vector3(clickPosition.x, clickPosition.y + canvas.sizeDelta.y / 2));
        }

        private Vector2 ClampStartPosition(Vector2 StartPosition)
        {
            /*if (StartPosition.x < JoystickSize.x / 2)
            {
                StartPosition.x = JoystickSize.x / 2;
            }
            if (StartPosition.y < JoystickSize.y / 2)
            {
                StartPosition.y = JoystickSize.y / 2;
            }
            else if (StartPosition.x > Screen.width - JoystickSize.x / 2)
            {
                StartPosition.x = Screen.width - JoystickSize.x / 2;
            }
            else if (StartPosition.y > Screen.height - JoystickSize.y / 2)
            {
                StartPosition.y = Screen.height - JoystickSize.y / 2;
            }*/
            return StartPosition;
        }

        public Vector3 scaledMovement;
        public Rigidbody rb;

        private void Update()
        {
            if (GameController.instance.poppies.Count == 0 || !navMeshAgent.enabled) return;
            scaledMovement = navMeshAgent.speed * Time.deltaTime * new Vector3(MovementAmount.x, 0, MovementAmount.y);
            navMeshAgent.Move(scaledMovement);
        }

        public bool IsCanMove()
        {
            NavMeshHit hit;
            return NavMesh.SamplePosition(navMeshAgent.transform.position + scaledMovement, out hit, 0.01f, NavMesh.AllAreas);
        }

        public void OnDestroy()
        {
            canvasGroup.DOKill();
        }
    }
}
