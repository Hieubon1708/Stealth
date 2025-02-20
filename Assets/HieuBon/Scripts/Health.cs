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
        float startHp;

        public void Awake()
        {
            startSize = healthBar.sizeDelta.x;
        }

        public void SubtractHp()
        {
            if (startHp == 0) startHp = bot.hp;
            healthBar.DOKill();
            healthDamagerBar.DOKill();
            healthBar.DOSizeDelta(new Vector2((float)bot.hp / startHp * startSize, healthBar.sizeDelta.y), 0.25f);
            healthDamagerBar.DOSizeDelta(new Vector2((float)bot.hp / startHp * startSize, healthDamagerBar.sizeDelta.y), 0.25f).SetDelay(0.25f);
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