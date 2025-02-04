using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Duc
{
    //[RequireComponent(typeof(Collider2D))]
    public class ButtonScale : Selectable, IPointerClickHandler, ISubmitHandler
    {
        //[SerializeField] private Transform target;
        //[SerializeField] private float time = 0.12f;
        //[SerializeField] private Vector3 size = 0.9f * Vector3.one;

        //[SerializeField] private UnityEvent onClick;

        //Vector3 sizeDown;
        //Vector3 cacheSizeBase;

        //bool isBusy;

        //// Start is called before the first frame update
        //void Start()
        //{
        //    if (target == null) target = transform;
        //    cacheSizeBase = transform.localScale;
        //    sizeDown = new Vector3(cacheSizeBase.x * size.x, cacheSizeBase.y * size.y, cacheSizeBase.z * size.z);
        //}


        //private void OnMouseDown()
        //{
        //    if (isBusy) return;
        //    isBusy = true;
        //    target.DOPause();
        //    target.localScale = cacheSizeBase;
        //    target.DOScale(sizeDown, time).SetEase(Ease.InOutSine).OnComplete(delegate { isBusy = false; });
        //}

        //private IEnumerator OnMouseUp()
        //{
        //    yield return new WaitWhile(() => isBusy);
        //    target.DOScale(cacheSizeBase, time).SetEase(Ease.InOutSine);
        //}

        //private void OnMouseUpAsButton()
        //{
        //    if (onClick != null) onClick.Invoke();
        //}

        [SerializeField] private Transform target;
        [SerializeField] private Vector3 size = 0.95f * Vector3.one;
        [SerializeField] private Vector3 cacheSizeBase;
        Vector3 sizeDown;

        [Serializable]
        /// <summary>
        /// Function definition for a button click event.
        /// </summary>
        public class ButtonClickedEvent : UnityEvent { }

        // Event delegates triggered on click.
        [FormerlySerializedAs("onClick")]
        [SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

        protected ButtonScale()
        { }

        /// <summary>
        /// UnityEvent that is triggered when the button is pressed.
        /// Note: Triggered on MouseUp after MouseDown on the same object.
        /// </summary>
        ///<example>
        ///<code>
        /// using UnityEngine;
        /// using UnityEngine.UI;
        /// using System.Collections;
        ///
        /// public class ClickExample : MonoBehaviour
        /// {
        ///     public Button yourButton;
        ///
        ///     void Start()
        ///     {
        ///         Button btn = yourButton.GetComponent<Button>();
        ///         btn.onClick.AddListener(TaskOnClick);
        ///     }
        ///
        ///     void TaskOnClick()
        ///     {
        ///         Debug.Log("You have clicked the button!");
        ///     }
        /// }
        ///</code>
        ///</example>
        public ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

        public void Active_Interactable(bool isActive)
        {
            interactable = isActive;
            if (isActive)
            {
                CanvasGroup canvasGroup;
                if (TryGetComponent(out canvasGroup))
                {
                    canvasGroup.alpha = 1;
                }
                else
                {
                    Image img = GetComponent<Image>();
                    img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
                }
            }
            else
            {
                CanvasGroup canvasGroup;
                if (TryGetComponent(out canvasGroup))
                {
                    canvasGroup.alpha = 0.5f;
                }
                else
                {
                    Image img = GetComponent<Image>();
                    img.color = new Color(img.color.r, img.color.g, img.color.b, 0.5f);
                }
            }
        }

        protected override void Start()
        {
            transition = Transition.None;
            if (target == null) target = transform;
            if (cacheSizeBase == Vector3.zero) cacheSizeBase = transform.localScale;
            sizeDown = new Vector3(cacheSizeBase.x * size.x, cacheSizeBase.y * size.y, cacheSizeBase.z * size.z);
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            transition = Transition.None;
            if (target == null) target = transform;
            if (cacheSizeBase == Vector3.zero) cacheSizeBase = transform.localScale;
            sizeDown = new Vector3(cacheSizeBase.x * size.x, cacheSizeBase.y * size.y, cacheSizeBase.z * size.z);
        }
#endif

        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;
            transition = Transition.None;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            m_OnClick.Invoke();

            if (AudioController.instance != null)
            {
                AudioController.instance.PlaySoundButtonClick();
            }
        }

        /// <summary>
        /// Call all registered IPointerClickHandlers.
        /// Register button presses using the IPointerClickHandler. You can also use it to tell what type of click happened (left, right etc.).
        /// Make sure your Scene has an EventSystem.
        /// </summary>
        /// <param name="eventData">Pointer Data associated with the event. Typically by the event system.</param>
        /// <example>
        /// <code>
        /// //Attatch this script to a Button GameObject
        /// using UnityEngine;
        /// using UnityEngine.EventSystems;
        ///
        /// public class Example : MonoBehaviour, IPointerClickHandler
        /// {
        ///     //Detect if a click occurs
        ///     public void OnPointerClick(PointerEventData pointerEventData)
        ///     {
        ///             //Use this to tell when the user right-clicks on the Button
        ///         if (pointerEventData.button == PointerEventData.InputButton.Right)
        ///         {
        ///             //Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
        ///             Debug.Log(name + " Game Object Right Clicked!");
        ///         }
        ///
        ///         //Use this to tell when the user left-clicks on the Button
        ///         if (pointerEventData.button == PointerEventData.InputButton.Left)
        ///         {
        ///             Debug.Log(name + " Game Object Left Clicked!");
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            Press();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            target.localScale = sizeDown;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            target.localScale = cacheSizeBase;
        }

        /// <summary>
        /// Call all registered ISubmitHandler.
        /// </summary>
        /// <param name="eventData">Associated data with the event. Typically by the event system.</param>
        /// <remarks>
        /// This detects when a Button has been selected via a "submit" key you specify (default is the return key).
        ///
        /// To change the submit key, either:
        ///
        /// 1. Go to Edit->Project Settings->Input.
        ///
        /// 2. Next, expand the Axes section and go to the Submit section if it exists.
        ///
        /// 3. If Submit doesn’t exist, add 1 number to the Size field. This creates a new section at the bottom. Expand the new section and change the Name field to “Submit”.
        ///
        /// 4. Change the Positive Button field to the key you want (e.g. space).
        ///
        ///
        /// Or:
        ///
        /// 1. Go to your EventSystem in your Project
        ///
        /// 2. Go to the Inspector window and change the Submit Button field to one of the sections in the Input Manager (e.g. "Submit"), or create your own by naming it what you like, then following the next few steps.
        ///
        /// 3. Go to Edit->Project Settings->Input to get to the Input Manager.
        ///
        /// 4. Expand the Axes section in the Inspector window. Add 1 to the number in the Size field. This creates a new section at the bottom.
        ///
        /// 5. Expand the new section and name it the same as the name you inserted in the Submit Button field in the EventSystem. Set the Positive Button field to the key you want (e.g. space)
        /// </remarks>

        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();

            // if we get set disabled during the press
            // don't run the coroutine.
            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }

        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }
    }
}