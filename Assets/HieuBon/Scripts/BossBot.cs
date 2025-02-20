using ACEPlay.Bridge;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Hunter
{
    public class BossBot : Bot
    {
        public int startHp;
        public GameObject arrow;
        Coroutine run;
        public Health health;
        public int amountBullet;
        public GameObject preBullet;
        public Bullet[] bullets;
        public int indexBullet;
        public float speedBullet;
        bool isRuning;
        public Transform startBullet;

        public void Start()
        {
            bullets = new Bullet[amountBullet];
            for (int i = 0; i < amountBullet; i++)
            {
                GameObject b = Instantiate(preBullet, GameController.instance.poolWeapon);
                b.SetActive(false);
                bullets[i] = b.GetComponent<Bullet>();
            }
            transform.LookAt(PlayerController.instance.transform, Vector3.up);
        }

        public void FixedUpdate()
        {
            if (!col.enabled || isRuning) return;
            if (radarView.target != null)
            {
                if (!navMeshAgent.isStopped)
                {
                    StopProbe();
                    radarView.SetColor(true);
                    navMeshAgent.isStopped = true;
                    animator.SetBool("Walking", false);
                }
                //transform.LookAt(radarView.target.transform.position);

                Vector3 targetDirection = radarView.target.transform.position - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 3.5f);
                float angle = Quaternion.Angle(transform.rotation, targetRotation);
                if (angle < 5)
                {
                    if (attack == null)
                    {
                        StartAttack(radarView.target);
                    }
                }

            }
            else
            {
                if (!isKilling && navMeshAgent.isStopped)
                {
                    //BridgeController.instance.Debug_Log("Start");
                    StopAttack();
                    StartProbe(index);
                    radarView.SetColor(false);
                    navMeshAgent.isStopped = false;
                    animator.SetBool("Walking", true);
                }
            }
        }

        public override IEnumerator Attack(GameObject poppy)
        {
            animator.SetTrigger("Aiming");
            animator.SetTrigger("Fire");
            yield return new WaitForSeconds(0.467f);
            weapon.Attack(poppy.transform);

            float Currentangle = -150f / 2;
            float angleIcrement = 150f / (5 - 1);
            float Sine;
            float Cosine;

            AudioController.instance.PlaySoundNVibrate(name.Contains("Swat") ? AudioController.instance.ak47Gun : AudioController.instance.laserGun, 0);

            for (int i = 0; i < 5; i++)
            {
                Sine = Mathf.Sin(Currentangle);
                Cosine = Mathf.Cos(Currentangle);

                bullets[indexBullet].gameObject.SetActive(false);
                bullets[indexBullet].rb.velocity = Vector3.zero;

                Vector3 dir = (transform.forward * Cosine) + (transform.right * Sine);
                bullets[indexBullet].transform.position = startBullet.position;
                bullets[indexBullet].transform.LookAt(bullets[indexBullet].transform.position + dir);

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

        void StartRun(Transform killer)
        {
            if (run == null)
            {
                run = StartCoroutine(Run(killer));
            }
        }

        void StopRun()
        {
            if (run != null)
            {
                StopCoroutine(run);
                run = null;
            }
        }

        public override void SubtractHp(int hp, Transform killer)
        {
            if (this.hp <= 0) return;
            this.hp = Mathf.Clamp(this.hp - hp, 0, this.hp);
            PlayBlood();
            StopProbe();
            StopAttack();
            health.SubtractHp();
            if (this.hp <= 0)
            {
                StopRun();
                AudioController.instance.PlaySoundNVibrate(AudioController.instance.enemyDie, 0);
                UIController.instance.camAni.Play("CamBossZoom");
                PlayerController.instance.playerTouchMovement.HandleLoseFinger();
                UIController.instance.layerCover.raycastTarget = true;
                UIController.instance.HitEffect();
                col.enabled = false;
                animator.enabled = false;
                navMeshAgent.enabled = false;
                radarView.gameObject.SetActive(false);
                arrow.SetActive(false);
                IsKinematic(false);
                BossEnd();
                Vector3 dir = (transform.position - PlayerController.instance.transform.position).normalized;
                for (int i = 0; i < rbs.Length; i++)
                {
                    rbs[i].AddForce(new Vector3(dir.x, dir.y, dir.z) * 10, ForceMode.Impulse);
                }
                GameController.instance.RemoveBot(gameObject);
                UIController.instance.gamePlay.UpdateRemainingEnemy();
            }
            else
            {
                AudioController.instance.PlaySoundNVibrate(AudioController.instance.enemyDamage, 0);
                if (isRuning) return;
                StopRun();
                StartRun(killer);
            }
            base.SubtractHp(hp, killer);
        }

        void BossEnd()
        {
            UIController.instance.BossEnd();
        }

        IEnumerator Run(Transform killer)
        {
            BridgeController.instance.Debug_Log("Run");
            isRuning = true;
            indexPath++;
            navMeshAgent.isStopped = false;
            radarView.SetColor(false);
            ChangeSpeed(detectSpeed, rotateDetectSpeed);
            Vector3 dirOfAttack = transform.position - killer.position;
            navMeshAgent.destination = transform.position + dirOfAttack * 2;
            animator.SetBool("Walking", true);
            animator.SetTrigger("Dodging");
            yield return new WaitForSeconds(0.35f);
            navMeshAgent.destination = pathInfo.paths[indexPath][0];
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            while (col.enabled)
            {
                if (navMeshAgent.remainingDistance <= 0.1f) animator.SetBool("Walking", false);
                if (navMeshAgent.remainingDistance == navMeshAgent.stoppingDistance) break;
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(time);
            StartProbe(1);
            isRuning = false;
        }

        public override void ResetBot()
        {
            radarView.SetColor(false);
            arrow.SetActive(true);
            hp = startHp;
            indexPath = 0;
            IsKinematic(true);
            col.enabled = true;
            animator.enabled = true;
            isKilling = false;
            navMeshAgent.enabled = true;
            navMeshAgent.isStopped = false;
            radarView.gameObject.SetActive(true);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(pathInfo.paths[0][0], out hit, 100, NavMesh.AllAreas))
            {
                navMeshAgent.Warp(hit.position);
            }
            else BridgeController.instance.Debug_LogWarning("!");
            transform.LookAt(pathInfo.paths[0][1], Vector3.up);
            navMeshAgent.destination = transform.position;
        }

        private void OnDestroy()
        {
            for (int i = 0; i < bullets.Length; i++)
            {
                Destroy(bullets[i]);
            }
        }
    }
}
