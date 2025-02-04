using UnityEngine;

namespace Hunter
{
    public class Dollar : MonoBehaviour
    {
        public Rigidbody rb;
        public GameObject target;
        public bool isOk;
        public bool isRotateLeft;
        public float forceRotate;
        Vector3 dir;
        public GameObject mesh;
        public ParticleSystem fx;
        public ClusterDollar clusterDollar;

        public void Out(Vector3 dir)
        {
            transform.localPosition = Vector3.zero;
            this.dir = dir;
            mesh.SetActive(true);
            rb.isKinematic = false;
            rb.excludeLayers = 0;
            rb.AddRelativeForce(dir, ForceMode.Impulse);
            isRotateLeft = Random.Range(0, 2) == 0;
            float randomForce = 20;
            forceRotate = isRotateLeft ? -randomForce : randomForce;
            rb.AddTorque(dir * forceRotate, ForceMode.Impulse);
        }

        public void In(GameObject target, LayerMask wallLayer)
        {
            this.target = target;
            fx.transform.SetParent(target.transform);
            fx.transform.localPosition = Vector3.zero;
            rb.excludeLayers = wallLayer;
            rb.AddTorque(dir * -forceRotate, ForceMode.Impulse);
            isOk = true;
        }

        public void FixedUpdate()
        {
            if (isOk)
            {
                Vector3 newDirection = Vector3.MoveTowards(rb.position, target.transform.position, 0.5f * Time.timeScale);
                rb.MovePosition(newDirection);
                if (Vector3.Distance(rb.position, target.transform.position) < 1f)
                {
                    isOk = false;
                    mesh.SetActive(false);
                    rb.isKinematic = true;
                    int coin = clusterDollar.GetCoin();
                    UIController.instance.gamePlay.UpdateCoin(coin);
                    fx.Play();
                }
            }
        }
        
        public void ResetFx()
        {
            fx.transform.SetParent(transform);
        }
    }
}