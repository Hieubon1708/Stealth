using UnityEngine;

namespace Hunter
{
    public class GlassBroken : ObjectBroken
    {
        public override void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Character") || collision.gameObject.CompareTag("Bullet") || collision.gameObject.CompareTag("BlastZone"))
            {
                AudioController.instance.PlaySoundNVibrate(AudioController.instance.glassBrocken, 0);
                gameObject.SetActive(false);
                for (int i = 0; i < rbs.Length; i++)
                {
                    rbs[i].gameObject.SetActive(true);
                    rbs[i].AddExplosionForce(500, collision.transform.position, Vector3.Distance(rbs[i].transform.position, collision.transform.position));
                }
                Invoke(nameof(Drop), 3.5f);
            }
        }

        public override void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Character") || other.CompareTag("Bullet") || other.CompareTag("BlastZone"))
            {
                AudioController.instance.PlaySoundNVibrate(AudioController.instance.glassBrocken, 0);
                gameObject.SetActive(false);
                for (int i = 0; i < rbs.Length; i++)
                {
                    rbs[i].gameObject.SetActive(true);
                    rbs[i].AddExplosionForce(500, other.transform.position, Vector3.Distance(rbs[i].transform.position, other.transform.position));
                }
                Invoke(nameof(Drop), 3.5f);
            }
        }
    }
}
