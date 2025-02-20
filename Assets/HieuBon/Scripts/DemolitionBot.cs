using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Hunter
{
    public class DemolitionBot : SentryBot
    {
        public GameObject preBoom;
        public GameObject boomOnHand;
        public int amount;
        int indexx;
        DemolitionWeapon[] demolitionWeapons;
        public Vector3 destination;
        public DemolitionWeapon onHand;
        bool isThrowing;
        public Transform hand;
        DemolitionWeapon demolitionWeapon;
        public override void Start()
        {
            demolitionWeapons = new DemolitionWeapon[amount];
            for (int i = 0; i < amount; i++)
            {
                GameObject b = Instantiate(preBoom, GameController.instance.poolWeapon);
                b.SetActive(false);
                demolitionWeapons[i] = b.GetComponent<DemolitionWeapon>();
            }
            base.Start();
        }

        public void Throw()
        {
            if (isThrowing) return;
            isThrowing = true;

            demolitionWeapon = demolitionWeapons[indexx];

            demolitionWeapon.DOKill();
            boomOnHand.transform.localScale = Vector3.zero;
            Vector3 dir = (destination - transform.position).normalized;

            demolitionWeapon.transform.localRotation = Quaternion.identity;
            demolitionWeapon.transform.position = weapon.transform.position;
            demolitionWeapon.ResetWeapon();
            demolitionWeapon.gameObject.SetActive(true);
            demolitionWeapon.transform.DOJump(destination, 3, 1, 0.5f).SetEase(Ease.Linear).OnComplete(delegate
            {
                demolitionWeapon.Timer();
            });
            demolitionWeapon.transform.DOLocalRotate(new Vector3(90, 0, 90), 0.5f).SetEase(Ease.Linear);

            indexx++;
            if (indexx == demolitionWeapons.Length) indexx = 0;
            boomOnHand.transform.DOScale(4f, 0.25f).SetDelay(0.5f).OnComplete(delegate
            {
                isThrowing = false;
                //StopAttack(); 
            });
        }

        public override void SubtractHp(int hp, Transform killer)
        {
            base.SubtractHp(hp, killer);
            onHand.rb.isKinematic = true;
            onHand.transform.SetParent(GameController.instance.poolWeapon);
            onHand.transform.DOKill();
            onHand.transform.localScale = Vector3.one;
            onHand.transform.DOJump(killer.position, 3, 1, 0.75f).SetEase(Ease.Linear).OnComplete(delegate
            {
                onHand.Timer();
            });
            onHand.transform.DOLocalRotate(new Vector3(90, 0, 90), 0.75f).SetEase(Ease.Linear);
        }

        public override void ResetBot()
        {
            onHand.transform.SetParent(hand);
            base.ResetBot();
        }

        public override IEnumerator Attack(GameObject target)
        {
            destination = target.transform.position;
            animator.SetTrigger("Fire");
            yield return null;
        }

        private void OnDestroy()
        {
            boomOnHand.transform.DOKill();
        }
    }
}