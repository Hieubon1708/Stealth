using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Hunter
{
    public class BatmanBoss : BossBot
    {
        public GameObject mainBomerang;

        public override IEnumerator Attack(GameObject poppy)
        {

            animator.SetTrigger("Aiming");
            animator.SetTrigger("Fire");
            yield return new WaitForSeconds(0.467f);

            float Currentangle = -150f / 2;
            float angleIcrement = 150f / (3 - 1);
            float Sine;
            float Cosine;
            mainBomerang.transform.localScale = Vector3.zero;

            AudioController.instance.PlaySoundNVibrate(AudioController.instance.boomerang, 0);
            for (int i = 0; i < 3; i++)
            {
                Sine = Mathf.Sin(Currentangle);
                Cosine = Mathf.Cos(Currentangle);

                bullets[indexBullet].gameObject.SetActive(false);
                bullets[indexBullet].rb.velocity = Vector3.zero;

                Vector3 dir = (transform.forward * Cosine) + (transform.right * Sine);
                bullets[indexBullet].transform.position = startBullet.position;
                bullets[indexBullet].transform.LookAt(bullets[indexBullet].transform.position + dir);
                bullets[indexBullet].transform.rotation = Quaternion.Euler(bullets[indexBullet].transform.localEulerAngles.x, bullets[indexBullet].transform.localEulerAngles.y, 90);
                bullets[indexBullet].rb.velocity = dir * speedBullet;
                bullets[indexBullet].trailRenderer.Clear();
                bullets[indexBullet].gameObject.SetActive(true);

                indexBullet++;
                if (indexBullet == bullets.Length) indexBullet = 0;

                Currentangle += angleIcrement;
            }
            yield return new WaitForSeconds(0.467f);
            mainBomerang.transform.DOScale(1.3f, 0.25f).SetEase(Ease.Linear);
            StopAttack();
        }
    }
}
