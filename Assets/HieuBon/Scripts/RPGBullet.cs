using DG.Tweening;
using System;
using UnityEngine;

namespace Hunter
{
    public class RPGBullet : MonoBehaviour
    {
        public Rigidbody rb;
        public TrailRenderer trailRenderer;
        Vector3 lastPosition;
        public ParticleSystem par;
        public GameObject mesh;
        public SphereCollider col;

        private void Update()
        {
            if (!mesh.activeSelf) return;
            Vector3 dir = transform.position - lastPosition;
            transform.rotation = Quaternion.LookRotation(dir);
            lastPosition = transform.position;
        }

        public void Init(Vector3 position)
        {
            col.enabled = false;
            gameObject.SetActive(false);
            mesh.SetActive(true);
            transform.position = position;
            transform.rotation = Quaternion.identity;
            trailRenderer.Clear();
            gameObject.SetActive(true);
        }

        public void OnGround()
        {
            mesh.SetActive(false);
            par.Play();
            col.enabled = true;
            trailRenderer.Clear();
            DOVirtual.DelayedCall(0.02f, delegate
            {
                col.enabled = false;
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
    }
}
