using UnityEngine;

namespace Hunter
{
    public class CamEvent : MonoBehaviour
    {
        public void SetTimeScale(float time)
        {
            Time.timeScale = time;
        }
    }
}