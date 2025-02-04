using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Hunter
{
    public class DemolitionWeapon : BotWeapon
    {
        public GameObject circle1;
        public GameObject circle2;
        public Image circle3;
        public float time;
        public ParticleSystem par;
        public SphereCollider col;
        public GameObject boom;
        public Rigidbody rb;

        private void Start()
        {
            par.transform.SetParent(GameController.instance.poolWeapon);
        }

        public override void Attack(Transform target)
        {

        }

        public void Timer()
        {
            circle1.transform.DOScale(3f, 0.25f).SetEase(Ease.Linear);
            circle2.transform.DOScale(0.55f, 0.75f).SetEase(Ease.OutBack);
            circle3.DOFillAmount(0f, time).SetEase(Ease.Linear).OnComplete(delegate
            {
                AudioController.instance.PlaySoundNVibrate(AudioController.instance.blastBarrel, 0);

                par.transform.position = transform.position;
                par.Play();
                col.enabled = true;
                DOVirtual.DelayedCall(0.02f, delegate
                {
                    col.enabled = false;
                });
                UIController.instance.virtualCam.StartShakeCam(2.5f);
                boom.SetActive(false);
                circle1.SetActive(false);
                circle2.SetActive(false);
            });
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Player player = GameController.instance.GetPoppy(other.gameObject);
                player.Die(transform);
            }
        }

        private void Update()
        {
            circle2.transform.LookAt(new Vector3(circle2.transform.position.x, GameController.instance.cam.transform.position.y, GameController.instance.cam.transform.position.z));
        }

        public void ResetWeapon()
        {
            boom.SetActive(true);
            circle1.SetActive(true);
            circle2.SetActive(true);
            circle1.transform.localScale = Vector3.zero;
            circle2.transform.localScale = Vector3.zero;
            circle3.fillAmount = 1f;
        }
    }
}