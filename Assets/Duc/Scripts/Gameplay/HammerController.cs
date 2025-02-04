using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;

namespace Duc.PoppyTangle
{
    public class HammerController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private ParticleSystem vfxHit;

        public void Play()
        {
            PinController pinToUnlock = (PinController)EventManager.GetData(EventVariables.BreakLockedPin);

            animator.Play("Base Layer.Idle");
            transform.position = new Vector3(2.5f, -6f, 0f);
            transform.DOMove(pinToUnlock.transform.position, 0.5f).OnComplete(() =>
            {
                animator.Play("Base Layer.Smash");
            });
        }

        public void OnSmash()
        {
            PinController pinToUnlock = (PinController)EventManager.GetData(EventVariables.BreakLockedPin);

            pinToUnlock.SetLock(false);

            vfxHit.Play();

            AudioController.instance.PlaySoundBoosterHammer();
        }

        public void OnSmashDone()
        {
            DOVirtual.DelayedCall(0.5f, () =>
            {
                gameObject.Recycle();
            });
        }
    }
}