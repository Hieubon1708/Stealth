using UnityEngine;

namespace Duc
{
    public abstract class UI : MonoBehaviour
    {
        [SerializeField] bool canCache = true;
        RectTransform rect;

        // Start is called before the first frame update
        public virtual void Start()
        {
            rect = GetComponent<RectTransform>();
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
        }

        public virtual void Hide()
        {
            if (!canCache)
            {
                Destroy(gameObject);
            }
        }
    }
}