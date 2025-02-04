using System.Collections;
using UnityEngine;

namespace Hunter
{
    public class NormalBot : SentryBot
    {
        public override IEnumerator Attack(GameObject target)
        {
            Player player = GameController.instance.GetPoppy(target);
            animator.SetTrigger("Aiming");
            animator.SetTrigger("Fire");
            yield return new WaitForSeconds(0.467f);
            player.Die(transform);
            weapon.Attack(player.transform);
            AudioController.instance.PlaySoundNVibrate(AudioController.instance.ak47Gun, 0);
            yield return new WaitForSeconds(0.467f);
            StopAttack();
        }
    }
}
