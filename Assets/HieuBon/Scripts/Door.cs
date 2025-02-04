using DG.Tweening;
using UnityEngine;

namespace Hunter
{
    public class Door : MonoBehaviour
    {
        public Rigidbody[] locks;
        public BoxCollider col;
        public ParticleSystem open;
        public ParticleSystem parLock;
        public GameObject door;
        public Key key;

        public void OpenDoor()
        {
            parLock.Stop();
            locks[0].isKinematic = false;
            locks[1].isKinematic = false;
            col.enabled = false;
            open.Play();
            door.transform.DOLocalRotate(Vector3.up * 90, 0.5f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (PlayerController.instance.IsKey(key))
                {
                    AudioController.instance.PlaySoundNVibrate(AudioController.instance.openDoor, 0);
                    OpenDoor();
                    key.gameObject.SetActive(false);
                }
            }
        }

        public void ResetDoor()
        {
            locks[0].isKinematic = true;
            locks[1].isKinematic = true;
            col.enabled = true;
            door.transform.localRotation = Quaternion.identity;
            key.ResetKey();
        }
    }
}
