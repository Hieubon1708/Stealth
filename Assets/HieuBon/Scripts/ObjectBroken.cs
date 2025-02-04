using UnityEngine;

namespace Hunter
{
    public class ObjectBroken : MonoBehaviour
    {
        public Rigidbody[] rbs;
        public MeshCollider[] cols;
        Vector3[] startPos;
        public float force;

        public void Start()
        {
            startPos = new Vector3[rbs.Length];
            for (int i = 0; i < rbs.Length; i++)
            {
                startPos[i] = rbs[i].transform.localPosition;
            }
        }

        public virtual void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Character") || collision.gameObject.layer == LayerMask.NameToLayer("Weapon"))
            {
                AudioController.instance.PlaySoundNVibrate(name.Contains("flower") ? AudioController.instance.flowerPotBrocken : AudioController.instance.objectBrocken, 0);
                gameObject.SetActive(false);
                for (int i = 0; i < rbs.Length; i++)
                {
                    rbs[i].gameObject.SetActive(true);
                    rbs[i].AddExplosionForce(500, collision.transform.position, Vector3.Distance(rbs[i].transform.position, collision.transform.position));
                }
                Invoke(nameof(Drop), 3.5f);
            }
        }

        public virtual void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Character") || other.gameObject.layer == LayerMask.NameToLayer("Weapon") || other.CompareTag("Player") || other.CompareTag("Bot"))
            {
                AudioController.instance.PlaySoundNVibrate(name.Contains("flower") ? AudioController.instance.flowerPotBrocken : AudioController.instance.objectBrocken, 0);
                gameObject.SetActive(false);
                for (int i = 0; i < rbs.Length; i++)
                {
                    rbs[i].gameObject.SetActive(true);
                    rbs[i].AddExplosionForce(500, other.transform.position, Vector3.Distance(rbs[i].transform.position, other.transform.position));
                }
                Invoke(nameof(Drop), 3.5f);
            }
        }

        public void Drop()
        {
            for (int i = 0; i < cols.Length; i++)
            {
                cols[i].enabled = false;
            }
            Invoke(nameof(Hide), 3.5f);
        }

        void Hide()
        {
            for (int i = 0; i < rbs.Length; i++)
            {
                rbs[i].gameObject.SetActive(false);
            }
        }

        public void ResetObject()
        {
            CancelInvoke(nameof(Drop));
            CancelInvoke(nameof(Hide));
            gameObject.SetActive(true);
            for (int i = 0; i < rbs.Length; i++)
            {
                rbs[i].velocity = Vector3.zero;
                rbs[i].gameObject.SetActive(false);
                rbs[i].transform.localPosition = startPos[i];
            }
            for (int i = 0; i < cols.Length; i++)
            {
                cols[i].enabled = true;
            }
        }
    }
}
