using UnityEngine;

namespace Hunter
{
    public class Electric : MonoBehaviour
    {
        public ParticleSystem par;
        public ParticleSystem fxStop;
        public ParticleSystem dealth;
        public float timeOn;
        public float timeOff;
        public BoxCollider col;

        void Start()
        {
            StartElectric();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                dealth.Play();
                Player player = GameController.instance.GetPoppy(other.gameObject);
                player.Die(transform);
            }
        }

        void StartElectric()
        {
            par.Play();
            Invoke(nameof(StopElectric), timeOn);
            Invoke(nameof(ColOn), 1f);
        }

        void ColOn()
        {
            col.enabled = true;
        }

        void StopElectric()
        {
            col.enabled = false;
            par.Stop();
            fxStop.Play();
            Invoke(nameof(StartElectric), timeOff);
        }
    }
}
