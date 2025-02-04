using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hunter
{
    public class Turrel : MonoBehaviour
    {
        public List<GameObject> poppies = new List<GameObject>();
        Coroutine shot;
        Coroutine rotateBody;
        public float[] angles;
        public SpriteRenderer spot;
        int indexPoint = 1;
        public ParticleSystem light1;
        public ParticleSystem light2;
        public SpriteRenderer laser1;
        public SpriteRenderer laser2;
        public GameObject gun1;
        public GameObject gun2;
        public GameObject body;

        private void Start()
        {
            transform.rotation = Quaternion.Euler(0, angles[0], 0);
        }

        public void Play()
        {
            Rotate();
        }

        public void ResetTurrel()
        {
            poppies.Clear();
            transform.DOKill();
            transform.localRotation = Quaternion.Euler(0, angles[0], 0);
            indexPoint = 1;
            spot.color = Color.white;
            if (shot != null) StopCoroutine(shot);
            if (rotateBody != null) StopCoroutine(rotateBody);
            rotateBody = null;
            shot = null;
        }

        void Rotate()
        {
            transform.DOLocalRotate(new Vector3(0, angles[indexPoint], 0), 2f, RotateMode.Fast).OnComplete(delegate
            {
                indexPoint++;
                if (indexPoint == angles.Length) indexPoint = 0;
                Rotate();
            }).SetEase(Ease.Linear).SetDelay(0.5f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (!poppies.Contains(other.gameObject))
                {
                    poppies.Add(other.gameObject);
                }
                if (shot == null)
                {
                    AudioController.instance.PlayTurrel();
                    spot.color = new Color(1, 0, 0, 0.5f);
                    transform.DOPause();
                    shot = StartCoroutine(Shot());
                    rotateBody = StartCoroutine(RotateBody());
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (poppies.Contains(other.gameObject))
                {
                    poppies.Remove(other.gameObject);
                }
                if (poppies.Count == 0)
                {
                    Resum();
                }
            }
        }


        void Resum()
        {
            AudioController.instance.StopTurrel();
            spot.color = Color.white;
            transform.DOPlay();
            if (shot != null) StopCoroutine(shot);
            if (rotateBody != null) StopCoroutine(rotateBody);
            rotateBody = null;
            shot = null;
        }

        IEnumerator RotateBody()
        {
            yield return null;
        }

        IEnumerator Shot()
        {
            while (poppies.Count != 0)
            {
                Player player = GameController.instance.GetPoppy(poppies[0]);
                yield return new WaitForSeconds(0.15f);
                light1.Play();
                gun1.transform.DOLocalMoveZ(0.3f, 0.15f / 2).SetEase(Ease.Linear).OnComplete(delegate
                {
                    gun1.transform.DOLocalMoveZ(-0.0349713f, 0.15f / 2).SetEase(Ease.Linear);
                });
                laser1.DOFade(30f / 255f, 0.15f / 2).SetEase(Ease.Linear).OnComplete(delegate
                {
                    laser1.DOFade(0f, 0.15f / 2).SetEase(Ease.Linear);
                });
                player.Die(transform);
                poppies.RemoveAt(0);
                yield return new WaitForSeconds(0.15f);
                light2.Play();
                gun2.transform.DOLocalMoveZ(0.3f, 0.15f / 2).SetEase(Ease.Linear).OnComplete(delegate
                {
                    gun2.transform.DOLocalMoveZ(-0.0349713f, 0.15f / 2f).SetEase(Ease.Linear);
                });
                laser2.DOFade(30f / 255f, 0.15f / 2).SetEase(Ease.Linear).OnComplete(delegate
                {
                    laser2.DOFade(0f, 0.15f / 2).SetEase(Ease.Linear);
                });
            }
            Resum();
        }

        void DoKill()
        {
            transform.DOKill();
            gun1.transform.DOKill();
            gun2.transform.DOKill();
            laser1.transform.DOKill();
            laser2.transform.DOKill();
        }

        private void OnDestroy()
        {
            DoKill();
        }
    }
}
