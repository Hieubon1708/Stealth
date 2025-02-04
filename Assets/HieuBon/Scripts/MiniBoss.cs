using ACEPlay.Bridge;
using System.Collections;
using UnityEngine;

namespace Hunter
{
    public class MiniBoss : SentryBot
    {
        public int amountBullet;
        public GameObject preBullet;
        public Bullet[] bullets;
        public int indexBullet;
        public float speedBullet;
        Coroutine dodging;
        public Health health;

        public void Start()
        {
            bullets = new Bullet[amountBullet];
            for (int i = 0; i < amountBullet; i++)
            {
                GameObject b = Instantiate(preBullet, GameController.instance.poolWeapon);
                bullets[i] = b.GetComponent<Bullet>();
                b.SetActive(false);
            }
        }

        void StartDodging(Transform killer)
        {
            if (dodging == null)
            {
                dodging = StartCoroutine(Dodging(killer));
            }
        }

        void StopDodging()
        {
            if (dodging != null)
            {
                StopCoroutine(dodging);
                dodging = null;
            }
        }

        public override IEnumerator Attack(GameObject target)
        {
            animator.SetTrigger("Aiming");
            animator.SetTrigger("Fire");
            while(target != null)
            {
                AudioController.instance.PlaySoundNVibrate(AudioController.instance.shotGun, 0);
                weapon.Attack(target.transform);
                bullets[indexBullet].gameObject.SetActive(false);
                bullets[indexBullet].rb.velocity = Vector3.zero;

                Vector3 dir = (target.transform.position - transform.position).normalized;
                bullets[indexBullet].transform.position = weapon.startBullet.transform.position;
                bullets[indexBullet].rb.velocity = dir * speedBullet;
                bullets[indexBullet].transform.LookAt(bullets[indexBullet].transform.position + dir);
                bullets[indexBullet].trailRenderer.Clear();
                bullets[indexBullet].gameObject.SetActive(true);
                indexBullet++;
                if (indexBullet == bullets.Length) indexBullet = 0;
                yield return new WaitForSeconds(0.467f * 1.5f);
            }
            StopAttack();
        }

        IEnumerator Dodging(Transform killer)
        {
            isDodging = true;
            isFind = true;
            navMeshAgent.isStopped = false;
            radarView.SetColor(false);
            ChangeSpeed(detectSpeed, rotateDetectSpeed);
            Vector3 dirOfAttack = transform.position - killer.position;
            navMeshAgent.destination = transform.position + dirOfAttack * 2;
            animator.SetBool("Walking", true);
            animator.SetTrigger("Dodging");
            yield return new WaitForSeconds(0.25f);
            animator.SetBool("Walking", false);
            target = killer.gameObject;
            BridgeController.instance.Debug_Log("Dodging");
            isDodging = false;
            StopDodging();
        }

        public override void SubtractHp(int hp, Transform killer)
        {
            if (this.hp <= 0) return;
            PlayBlood();
            StopProbe();
            StopAttack();
            base.SubtractHp(hp, killer);
            if(this.hp == 0)
            {
                GameController.instance.map.GetComponentInChildren<EndDoor>().col.enabled = true;
                StopDodging();
                health.gameObject.SetActive(false);
            }
            else
            {
                StartDodging(killer);
            }
            health.SubtractHp();
        }
    }
}
