using UnityEngine;

namespace Hunter
{
    public class QC : MonoBehaviour
    {
        public GameObject[] canvases;
        public CanvasGroup touch;
        bool isActive;

        void Update()
        {
            /*if (Input.GetKeyDown(KeyCode.U))
            {
                for (int i = 0; i < canvases.Length; i++)
                {
                    canvases[i].SetActive(isActive);
                }
                touch.alpha = isActive ? 1 : 0;
                UIEconomy.instance.cashContainer.SetActive(isActive);
                isActive = !isActive;
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                UIController.instance.gamePlay.ChangeWeapon(0);
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                UIController.instance.gamePlay.ChangeWeapon(1);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                UIController.instance.Win();
                UIController.instance.ChangeMap();
            }*/
        }
    }
}
