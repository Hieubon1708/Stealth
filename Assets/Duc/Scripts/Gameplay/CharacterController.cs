using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duc
{
    public class CharacterController : MonoBehaviour
    {
        public Character character;
        [SerializeField] private Animator anim;

        private void Awake()
        {
            if (anim == null) anim = GetComponent<Animator>();
        }

        public void Dance()
        {
            anim.Play($"Base Layer.Dance_{Random.Range(1, 9)}", 0, 1);
        }

        public void Run()
        {
            anim.Play("Base Layer.Run", 0, 1);
            transform.DOLocalMoveZ(-25f, 20f).SetSpeedBased().SetEase(Ease.Linear);
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }
    }
}