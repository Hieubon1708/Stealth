using System.Collections;
using UnityEngine;

namespace Hunter
{
    public class VenomBoss : BossBot
    {
        public override IEnumerator Attack(GameObject poppy)
        {
            animator.SetTrigger("Aiming");
            animator.SetTrigger("Fire");
            yield return new WaitForSeconds(0.467f);

            while (radarView.target != null)
            {
                AudioController.instance.PlaySoundNVibrate(AudioController.instance.shotGun, 0);
                bullets[indexBullet].gameObject.SetActive(false);
                bullets[indexBullet].rb.velocity = Vector3.zero;

                Vector3 dir = (poppy.transform.position - transform.position).normalized;
                bullets[indexBullet].transform.position = startBullet.position;
                bullets[indexBullet].rb.velocity = dir * speedBullet;
                bullets[indexBullet].transform.LookAt(poppy.transform.position);
                bullets[indexBullet].trailRenderer.Clear();
                bullets[indexBullet].gameObject.SetActive(true);
                weapon.Attack(poppy.transform);

                indexBullet++;
                if (indexBullet == bullets.Length) indexBullet = 0;

                yield return new WaitForSeconds(0.467f);
            }
            StopAttack();
        }
    }
}
