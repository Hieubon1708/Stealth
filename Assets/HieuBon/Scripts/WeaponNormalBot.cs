using DG.Tweening;
using UnityEngine;

namespace Hunter
{
    public class WeaponNormalBot : BotWeapon
    {
        public SpriteRenderer laser1;
        public SpriteRenderer laser2;
        public BoxCollider col;

        public override void Attack(Transform target)
        {
            parWeapon.Play();
            col.enabled = true;
            laser1.transform.localScale = new Vector3(laser1.transform.localScale.x, Vector3.Distance(transform.position, new Vector3(target.position.x, transform.position.y, target.position.z)) * 4, laser1.transform.localScale.z);
            laser2.transform.localScale = new Vector3(laser2.transform.localScale.x, Vector3.Distance(transform.position, new Vector3(target.position.x, transform.position.y, target.position.z)) * 4, laser2.transform.localScale.z);
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
            laser1.DOFade(30f / 255f, 0.05f).OnComplete(delegate
            {
                col.enabled = false;
                laser1.DOFade(0, 0.1f);
            });
            laser2.DOFade(30f / 255f, 0.05f).OnComplete(delegate
            {
                laser2.DOFade(0, 0.1f);
            });
        }
    }
}
