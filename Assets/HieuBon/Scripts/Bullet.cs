using UnityEngine;

namespace Hunter
{
    public class Bullet : MonoBehaviour
    {
        public Rigidbody rb;
        public TrailRenderer trailRenderer;

        public virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                gameObject.SetActive(false);
                Player player = GameController.instance.GetPoppy(other.gameObject);
                if (player != null)
                {
                    player.Die(transform);
                }
            }
            if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                gameObject.SetActive(false);
            }
        }
    }
}
