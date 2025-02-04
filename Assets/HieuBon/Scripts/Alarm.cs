using UnityEngine;

namespace Hunter
{
    public class Alarm : MonoBehaviour
    {
        public ParticleSystem ligtht;

        public void Alert()
        {
            ligtht.Play();
        }

        public void StopAlert()
        {
            ligtht.Stop();
        }

        public void ResetBell()
        {
            StopAlert();
        }
    }
}
