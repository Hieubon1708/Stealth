using UnityEngine;
using DG.Tweening;

namespace Hunter
{
    public class WeaponRange : Weapon
    {
        public SpriteRenderer laser1;
        public SpriteRenderer laser2;
        public ParticleSystem parWeapon;
        public BoxCollider col;

        public override void Attack(Transform target)
        {
            laser1.DOKill();
            laser2.DOKill();
            laser1.color = new Color(1, 1, 1, 0);
            laser2.color = new Color(1, 1, 1, 0);
            parWeapon.Play();
            col.enabled = true;
            laser1.transform.localScale = new Vector3(laser1.transform.localScale.x, Vector3.Distance(transform.position, new Vector3(target.position.x, transform.position.y, target.position.z)) * 4, laser1.transform.localScale.z);
            laser2.transform.localScale = new Vector3(laser2.transform.localScale.x, Vector3.Distance(transform.position, new Vector3(target.position.x, transform.position.y, target.position.z)) * 4, laser2.transform.localScale.z);
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
            laser1.DOFade(70f / 255f, 0.05f).OnComplete(delegate
            {
                col.enabled = false;
                laser1.DOFade(0, 0.1f);
            });
            laser2.DOFade(70f / 255f, 0.05f).OnComplete(delegate
            {
                laser2.DOFade(0, 0.1f);
            });
        }
    }
}
