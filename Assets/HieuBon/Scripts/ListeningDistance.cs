using ACEPlay.Bridge;
using UnityEngine;

namespace Hunter
{
    public class ListeningDistance : MonoBehaviour
    {
        public SentryBot bot;

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Scream") && !bot.isFind && bot.col.enabled)
            {
                bot.StartHear(other.gameObject);
                BridgeController.instance.Debug_Log("Enter " + other.transform.parent.name + " position " + other.transform.position);
            }
        }
    }
}
