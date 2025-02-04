using UnityEngine;

namespace Hunter
{
    public class DollarOnGround : MonoBehaviour
    {
        public GameObject target;
        public bool isOk;
        public Rigidbody rb;
        public ParticleSystem fx;

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Player player = GameController.instance.GetPoppy(other.gameObject);
                target = player.fxPool;
                fx.transform.SetParent(target.transform);
                fx.transform.localPosition = Vector3.zero;
                isOk = true;
            }
        }

        public void FixedUpdate()
        {
            if (isOk)
            {
                Vector3 newDirection = Vector3.MoveTowards(rb.position, target.transform.position, 0.25f);
                rb.MovePosition(newDirection);
                if (Vector3.Distance(rb.position, target.transform.position) < 1f)
                {
                    isOk = false;
                    gameObject.SetActive(false);
                    fx.Play();
                    UIController.instance.gamePlay.UpdateCoin(1);
                }
            }
        }
    }
}
