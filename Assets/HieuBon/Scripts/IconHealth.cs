using UnityEngine;

namespace Hunter
{
    public class IconHealth : MonoBehaviour
    {
        private void Start()
        {
            
        }
        void Update()
        {
            if (GameController.instance != null)
            {
                transform.LookAt(new Vector3(transform.position.x, GameController.instance.cam.transform.position.y, GameController.instance.cam.transform.position.z));
            }
        }
    }
}
