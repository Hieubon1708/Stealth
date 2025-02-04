using UnityEngine;

namespace Hunter
{
    public class QuestionRotate : MonoBehaviour
    {
        public Bot bot;
        public Animation ani;

        public void Show()
        {
            if (!bot.col.enabled || transform.localScale != Vector3.zero) return;
            transform.localScale = Vector3.one * 1.5f;
            //ani.Play("ShowQuestion");
        }

        public void Hide()
        {
            if (transform.localScale == Vector3.zero) return;
            //ani.Play("HideQuestion");
            transform.localScale = Vector3.zero;
        }

        private void LateUpdate()
        {
            if (GameController.instance != null && transform.localScale != Vector3.zero)
            {
                transform.LookAt(new Vector3(transform.position.x, GameController.instance.cam.transform.position.y, GameController.instance.cam.transform.position.z));
            }
        }
    }
}
