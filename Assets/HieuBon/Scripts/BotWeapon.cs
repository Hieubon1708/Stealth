using UnityEngine;

namespace Hunter
{
    public abstract class BotWeapon : MonoBehaviour
    {
        public ParticleSystem parWeapon;
        public Transform startBullet;
        public abstract void Attack(Transform target);
    }
}
