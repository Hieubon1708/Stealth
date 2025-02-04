using UnityEngine;

namespace Hunter
{
    public class BossWeapon : BotWeapon
    {
        public override void Attack(Transform target)
        {
            parWeapon.Play();
        }
    }
}