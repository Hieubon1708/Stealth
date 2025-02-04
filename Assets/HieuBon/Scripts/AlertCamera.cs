using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Hunter
{
    public class AlertCamera : MonoBehaviour
    {
        public GameObject[] points;
        public SpriteRenderer spot;
        int indexPoint = 1;
        Vector3[] targetPoints;
        List<GameObject> bots = new List<GameObject>();
        List<GameObject> poppies = new List<GameObject>();

        private void Start()
        {
            targetPoints = new Vector3[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                targetPoints[i] = points[i].transform.position;
            }
            transform.LookAt(targetPoints[0]);
        }
        public void Play()
        {
            Rotate();
        }

        void Rotate()
        {
            transform.DOLookAt(targetPoints[indexPoint], 2f).OnComplete(delegate
            {
                indexPoint++;
                if (indexPoint == points.Length) indexPoint = 0;
                Rotate();
            }).SetEase(Ease.Linear).SetDelay(0.5f);
        }

        public void ResetAlertCamera()
        {
            bots.Clear();
            transform.DOKill();
            transform.LookAt(targetPoints[0]);
            spot.color = Color.white;
            indexPoint = 1;
            Rotate();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !GameController.instance.isAlert)
            {
                CancelInvoke(nameof(ResumScan));
                spot.color = new Color(1, 0, 0, 0.5f);
                transform.DOPause();
                GameController.instance.Alert(GameController.AlertType.Camera, gameObject);
                Invoke(nameof(CheckRemainingPoppyInSpot), 3f);
            }
            if (other.attachedRigidbody != null && other.name.Contains("Hip") & other.transform.parent.name == "Die")
            {
                if (!bots.Contains(other.attachedRigidbody.gameObject))
                {
                    CancelInvoke(nameof(ResumScan));
                    bots.Add(other.attachedRigidbody.gameObject);
                    GameController.instance.Alert(GameController.AlertType.Camera, gameObject);
                    spot.color = new Color(1, 0, 0, 0.5f);
                    transform.DOPause();
                    Invoke(nameof(ResumScan), 2f);
                }
            }
        }

        void CheckRemainingPoppyInSpot()
        {
            bool isRemaining = false;
            for (int i = 0; i < poppies.Count; i++)
            {
                for (int j = 0; j < GameController.instance.poppies.Count; j++)
                {
                    if (poppies[i] == GameController.instance.poppies[j].gameObject)
                    {
                        isRemaining = true;
                        break;
                    }
                }
                if (isRemaining) break;
                else poppies.RemoveAt(i);
            }
            if (!isRemaining) ResumScan();
            else Invoke(nameof(CheckRemainingPoppyInSpot), 1f);
        }

        void ResumScan()
        {
            spot.color = Color.white;
            transform.DOPlay();
            GameController.instance.isAlert = false;
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                poppies.Remove(other.gameObject);
                if (poppies.Count == 0)
                {
                    ResumScan();
                    CancelInvoke(nameof(CheckRemainingPoppyInSpot));
                }
            }
        }

        public void OnDestroy()
        {
            transform.DOKill();
        }
    }
}
