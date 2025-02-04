using DG.Tweening;
using UnityEngine;

namespace Hunter
{
    public class Laser : MonoBehaviour
    {
        public GameObject[] laserLines;
        public BoxCollider col;
        public GameObject button;

        public void LaserOn()
        {
            ActiveLaser(true);
        }

        public void LaserOff()
        {
            ActiveLaser(false);
        }

        void ActiveLaser(bool isActive)
        {
            button.transform.DOLocalMoveY(-0.2f, 0.25f).SetEase(Ease.Linear).OnComplete(delegate
            {
                for (int i = 0; i < laserLines.Length; i++)
                {
                    laserLines[i].SetActive(isActive);
                }
            });
            col.enabled = isActive;
        }

        public void ResetLaser()
        {
            LaserOn();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                LaserOff();            
            }
        }
    }
}
