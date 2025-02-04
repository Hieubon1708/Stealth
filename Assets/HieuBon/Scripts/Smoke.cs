using UnityEngine;

namespace Hunter
{
    public class Smoke : MonoBehaviour
    {
        public Material material1;
        public Material material2;
        public Renderer ren;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ren.enabled = false;
                GameController.instance.GetPoppy(other.gameObject).SetMaterial(material2);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ren.enabled = true;
                GameController.instance.GetPoppy(other.gameObject).SetMaterial(null);
            }
        }
    }
}
