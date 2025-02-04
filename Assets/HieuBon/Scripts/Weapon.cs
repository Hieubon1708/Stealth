using UnityEngine;

namespace Hunter
{
    public abstract class Weapon: MonoBehaviour
    {
        public GameController.WeaponType weaponType;

        public float attackRange;
        public int damage;
        public Outline outline;
        public MeshRenderer meshRenderer;
        public Material defaultMaterial;

        public abstract void Attack(Transform target);
    }
}
