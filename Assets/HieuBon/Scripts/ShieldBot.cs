using ACEPlay.Bridge;
using System.Collections;
using UnityEngine;

namespace Hunter
{
    public class ShieldBot : SentryBot
    {
        public SpriteRenderer healthBar1;
        public SpriteRenderer healthBar2;
        Coroutine dodging;

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
            Player player = GameController.instance.GetPoppy(target);
            animator.SetTrigger("Aiming");
            animator.SetTrigger("Fire");
            yield return new WaitForSeconds(0.467f);
            //player.Die(transform);
            weapon.Attack(player.transform);
            AudioController.instance.PlaySoundNVibrate(AudioController.instance.ak47Gun, 0);
            yield return new WaitForSeconds(0.467f);
            StopAttack();
        }

        IEnumerator Dodging(Transform killer)
        {
            isDodging = true;
            isFind = true;
            target = killer.gameObject;
            navMeshAgent.isStopped = false;
            radarView.SetColor(false);
            ChangeSpeed(detectSpeed, rotateDetectSpeed);
            Vector3 dirOfAttack = transform.position - killer.position;
            navMeshAgent.destination = transform.position + dirOfAttack * 2;
            animator.SetBool("Walking", true);
            animator.SetTrigger("Dodging");
            yield return new WaitForSeconds(0.5f);
            animator.SetBool("Walking", false);
            target = killer.gameObject;
            BridgeController.instance.Debug_Log("Dodging");
            isDodging = false;
        }

        public override void SubtractHp(int hp, Transform killer)
        {
            if (this.hp <= 0) return;
            PlayBlood();
            StopProbe();
            StopAttack();
            if (healthBar2.color != Color.black)
            {
                healthBar2.color = Color.black;
                StartDodging(killer);
            }
            else
            {
                healthBar1.enabled = false;
                healthBar2.enabled = false;
                StopDodging();
            }
            base.SubtractHp(hp, killer);
        }

        public override void ResetBot()
        {
            base.ResetBot();
            healthBar1.enabled = true;
            healthBar2.enabled = true;
            healthBar2.color = Color.white;
            StopDodging();
        }
    }
}
