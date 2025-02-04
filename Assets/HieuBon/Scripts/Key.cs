using DG.Tweening;
using UnityEngine;

namespace Hunter
{
    public class Key : MonoBehaviour
    {
        public ParticleSystem par1;
        public ParticleSystem par2;
        public ParticleSystem par3;
        public GameObject keyChild;
        Vector3 startPos;
        public SphereCollider col;

        private void Start()
        {
            startPos = transform.position;
            Rotate(gameObject, Vector3.up * 360);
        }

        void Rotate(GameObject obj, Vector3 endValue)
        {
            obj.transform.DOLocalRotate(endValue, 3f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                AudioController.instance.PlaySoundNVibrate(AudioController.instance.pickUp, 0);

                par1.Stop();
                par2.Stop();
                par3.Play();

                transform.DOKill();
                keyChild.transform.DOKill();
                transform.SetParent(other.transform);
                transform.localPosition = new Vector3(0, 12, 0);

                keyChild.transform.localPosition = Vector3.right * 0.05f;
                keyChild.transform.localScale = Vector3.one * 0.75f;
                keyChild.transform.localRotation = Quaternion.Euler(0, 0, -90);

                Rotate(keyChild, Vector3.right * 360);

                col.enabled = false;

                PlayerController.instance.SetKey(this, other.gameObject);
            }
        }

        private void LateUpdate()
        {
            if (keyChild.transform.localScale == Vector3.one) return;
            transform.LookAt(GameController.instance.cam.transform.position, Vector3.up);
        }

        public void ResetKey()
        {
            gameObject.SetActive(true);

            par1.Play();
            par2.Play();
            par3.Stop();

            transform.DOKill();
            keyChild.transform.DOKill();
            transform.SetParent(GameController.instance.map.transform);
            transform.localPosition = new Vector3(transform.localPosition.x, 3.5f, transform.localPosition.z);
            keyChild.transform.localRotation = Quaternion.Euler(0f, 0f, -40f);
            keyChild.transform.localScale = Vector3.one;
            keyChild.transform.localPosition = Vector3.zero;

            col.enabled = true;

            Rotate(gameObject, Vector3.up * 360);
        }

        private void OnDestroy()
        {
            transform.DOKill();
            keyChild.transform.DOKill();
        }
    }
}