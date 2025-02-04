using DG.Tweening;
using Exploder.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Duc
{
    public class Wall : MonoBehaviour
    {
        [Header("Move")]
        [SerializeField] private bool move;
        [SerializeField] private Transform targetMove;
        [SerializeField] private Vector3 moveTo;
        [SerializeField] private float moveTime;

        [Header("Break")]
        public bool isBreak;
        [SerializeField] private List<GameObject> objsToBreak;

        [SerializeField] private List<GameObject> objsDisable;

        public void Action(Action onDone = null)
        {
            objsDisable.ForEach(o => o.SetActive(false));

            if (move)
            {
                targetMove.DOMove(moveTo, moveTime).OnComplete(() =>
                {
                    onDone?.Invoke();
                });
            }
            else if (isBreak)
            {
                foreach (var obj in objsToBreak)
                {
                    ExploderSingleton.Instance.ExplodeObject(obj);

                    DOVirtual.DelayedCall(0.5f, () =>
                    {
                        onDone?.Invoke();
                    });
                }
            }
        }
    }
}