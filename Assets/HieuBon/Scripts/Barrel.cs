using DG.Tweening;
using UnityEngine;

namespace Hunter
{
    public class Barrel : ObjectBroken
    {
        public ParticleSystem bummm;
        public GameObject affectedArea;

        public override void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Character") && !collision.rigidbody.isKinematic || collision.gameObject.CompareTag("Bullet") || collision.gameObject.CompareTag("BlastZone"))
            {
                bummm.Play();
                DOVirtual.DelayedCall(0.1f, delegate
                {
                    AudioController.instance.PlaySoundNVibrate(AudioController.instance.blastBarrel, 0);
                    gameObject.SetActive(false);
                    for (int i = 0; i < rbs.Length; i++)
                    {
                        rbs[i].gameObject.SetActive(true);
                        rbs[i].AddExplosionForce(500, collision.transform.position, Vector3.Distance(rbs[i].transform.position, collision.transform.position));
                    }
                    Invoke(nameof(Drop), 3.5f);
                    affectedArea.SetActive(true);
                    DOVirtual.DelayedCall(0.1f, delegate
                    {
                        affectedArea.SetActive(false);
                    });
                });
            }
        }

        public override void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Character") && !other.attachedRigidbody.isKinematic || other.CompareTag("Bullet") || other.CompareTag("BlastZone"))
            {
                bummm.Play();
                DOVirtual.DelayedCall(0.1f, delegate
                {
                    AudioController.instance.PlaySoundNVibrate(AudioController.instance.blastBarrel, 0);
                    gameObject.SetActive(false);
                    for (int i = 0; i < rbs.Length; i++)
                    {
                        rbs[i].gameObject.SetActive(true);
                        rbs[i].AddExplosionForce(500, other.transform.position, Vector3.Distance(rbs[i].transform.position, other.transform.position));
                    }
                    Invoke(nameof(Drop), 3.5f);
                    affectedArea.SetActive(true);
                    DOVirtual.DelayedCall(0.1f, delegate
                    {
                        affectedArea.SetActive(false);
                    });
                });
            }

        }

        public void ResetBarrel()
        {
            affectedArea.SetActive(false);
        }
    }
}
