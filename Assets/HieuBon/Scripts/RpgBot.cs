using ACEPlay.Bridge;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Hunter
{
    public class RPGBot : Bot
    {
        public int amountBullet;
        public GameObject preBullet;
        public RPGBullet[] bullets;
        public int indexBullet;
        public float speedBullet;
        public float timeDelay;
        public SpriteRenderer healthBar1;
        public SpriteRenderer healthBar2;
        public SpriteRenderer healthBar3;
        Coroutine dodging;
        GameObject target;
        public ParticleSystem fxTarget;
        LayerMask playerLayer;
        public Collider[] targets;

        public void Start()
        {
            fxTarget.transform.SetParent(GameController.instance.map.transform);
            playerLayer = LayerMask.GetMask("Player");
            bullets = new RPGBullet[amountBullet];
            for (int i = 0; i < amountBullet; i++)
            {
                GameObject b = Instantiate(preBullet, GameController.instance.poolWeapon);
                bullets[i] = b.GetComponent<RPGBullet>();
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

        private void FixedUpdate()
        {
            if (isDodging || !col.enabled) return;
            targets = Physics.OverlapSphere(transform.position, 12.5f, playerLayer);
            if (targets.Length > 0)
            {
                StartAttack(null);
                transform.LookAt(target.transform);
            }
            else
            {
                StopAttack();
            }
        }

        public override IEnumerator Attack(GameObject target)
        {
            while (targets.Length > 0)
            {
                this.target = targets[0].gameObject;
                animator.SetTrigger("Fire");
                yield return new WaitForSeconds(timeDelay);
            }
        }

        public void Throw()
        {
            AudioController.instance.PlaySoundNVibrate(AudioController.instance.shotGun, 0);
            weapon.Attack(target.transform);
            bullets[indexBullet].Init(weapon.startBullet.position);
            int index = indexBullet;
            fxTarget.transform.position = target.transform.position;
            fxTarget.transform.DOScale(10f / 4f, 1f).SetEase(Ease.Linear).OnComplete(delegate
            {
                fxTarget.transform.localScale = Vector3.zero;
            });
            bullets[indexBullet].transform.DOJump(target.transform.position, 7, 1, 1f).SetEase(Ease.Linear).OnComplete(delegate
            {
                bullets[index].OnGround();
            });
            indexBullet++;
            if (indexBullet == bullets.Length) indexBullet = 0;
        }

        IEnumerator Dodging(Transform killer)
        {
            isDodging = true;
            StopAttack();
            navMeshAgent.isStopped = false;
            Vector3 dirOfAttack = transform.position - killer.position;
            navMeshAgent.destination = transform.position + dirOfAttack * 2;
            animator.SetBool("Walking", true);
            animator.SetTrigger("Dodging");
            yield return new WaitForSeconds(0.5f);
            navMeshAgent.isStopped = true;
            animator.SetBool("Walking", false);
            yield return new WaitForSeconds(0.5f);
            isDodging = false;
        }

        public override void SubtractHp(int hp, Transform killer)
        {
            if (this.hp <= 0) return;
            this.hp = Mathf.Clamp(this.hp - hp, 0, this.hp);
            PlayBlood();
            StopDodging();
            if (this.hp >= 200)
            {
                healthBar3.color = Color.black;
                StartDodging(killer);
            }
            else if (this.hp >= 100)
            {
                healthBar2.color = Color.black;
                StartDodging(killer);
            }
            if (this.hp <= 0)
            {
                healthBar1.enabled = false;
                healthBar2.enabled = false;
                healthBar3.enabled = false;
                AudioController.instance.PlaySoundNVibrate(AudioController.instance.enemyDie, 50);
                UIController.instance.HitEffect();
                UIController.instance.virtualCam.StartShakeCam(5);
                StopAttack();
                col.enabled = false;
                animator.enabled = false;
                IsKinematic(false);
                StartCoroutine(Die());

                Vector3 dir = transform.position - PlayerController.instance.transform.position;
                for (int i = 0; i < rbs.Length; i++)
                {
                    rbs[i].AddForce(new Vector3(dir.x, dir.y + 1, dir.z) * 7, ForceMode.Impulse);
                }

                GameController.instance.RemoveBot(gameObject);
                UIController.instance.gamePlay.UpdateRemainingEnemy();
            }
            else
            {
                AudioController.instance.PlaySoundNVibrate(AudioController.instance.enemyDamage, 50);
            }
            base.SubtractHp(hp, killer);
        }

        public override void ResetBot()
        {
            indexPath = 0;
            IsKinematic(true);
            col.enabled = true;
            animator.enabled = true;
            isKilling = false;
            navMeshAgent.enabled = true;
            navMeshAgent.isStopped = false;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(pathInfo.paths[0][0], out hit, 100, NavMesh.AllAreas))
            {
                navMeshAgent.Warp(hit.position);
            }
            else BridgeController.instance.Debug_LogWarning("!");
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, pathInfo.angle, transform.eulerAngles.z);
            navMeshAgent.destination = transform.position;

            healthBar1.enabled = true;
            healthBar2.enabled = true;
            healthBar3.enabled = true;
            healthBar2.color = Color.white;
            healthBar3.color = Color.white;
        }
    }
}
