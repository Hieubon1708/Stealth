using UnityEngine;

namespace Hunter
{
    public class WeaponRangeBot : BotWeapon
    {
        public override void Attack(Transform target)
        {
            parWeapon.Play();
        }
    }
}
