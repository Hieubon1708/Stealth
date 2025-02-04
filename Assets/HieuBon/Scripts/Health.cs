using DG.Tweening;
using UnityEngine;

namespace Hunter
{
    public class Health : MonoBehaviour
    {
        public RectTransform healthDamagerBar;
        public RectTransform healthBar;
        public Bot bot;
        float startSize;
        public bool isLookAtCamera;

        public void Awake()
        {
            startSize = healthBar.sizeDelta.x;
        }

        public void ResetHealth()
        {
            healthBar.sizeDelta = new Vector2(startSize, healthBar.sizeDelta.y);
            healthDamagerBar.sizeDelta = new Vector2(startSize, healthDamagerBar.sizeDelta.y);
        }

        public void SubtractHp()
        {
            healthBar.DOComplete();
            healthDamagerBar.DOComplete();
            healthBar.DOSizeDelta(new Vector2((float)bot.hp / bot.startHp * startSize, healthBar.sizeDelta.y), 0.25f);
            healthDamagerBar.DOSizeDelta(new Vector2((float)bot.hp / bot.startHp * startSize, healthDamagerBar.sizeDelta.y), 0.25f).SetDelay(0.25f);
        }

        private void OnDestroy()
        {
            healthBar.DOComplete();
            healthDamagerBar.DOComplete();
        }

        public void LateUpdate()
        {
            if (isLookAtCamera)
            {
                transform.LookAt(new Vector3(transform.position.x, GameController.instance.cam.transform.position.y, GameController.instance.cam.transform.position.z));
            }
        }
    }
}