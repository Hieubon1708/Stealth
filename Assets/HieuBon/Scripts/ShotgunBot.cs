using System.Collections;
using UnityEngine;

namespace Hunter
{
    public class ShotgunBot : SniperBot
    {
        public override IEnumerator Attack(GameObject target)
        {
            animator.SetTrigger("Aiming");
            yield return new WaitForSeconds(timeDelay);
            animator.SetTrigger("Fire");
            weapon.Attack(target.transform);

            float Currentangle = -150f / 2;
            float angleIcrement = 150f / (3 - 1);
            float Sine;
            float Cosine;

            AudioController.instance.PlaySoundNVibrate(AudioController.instance.shotGun, 0);
            for (int i = 0; i < 3; i++)
            {
                Sine = Mathf.Sin(Currentangle);
                Cosine = Mathf.Cos(Currentangle);

                bullets[indexBullet].gameObject.SetActive(false);
                bullets[indexBullet].rb.velocity = Vector3.zero;

                Vector3 dir = (transform.forward * Cosine) + (transform.right * Sine);
                bullets[indexBullet].transform.position = weapon.startBullet.position;
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
            StopAttack();
        }
    }
}
