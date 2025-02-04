using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hunter
{
    public class StartDoor : MonoBehaviour
    {
        public GameObject leftDoor;
        public GameObject rightDoor;
        public float targetXLeft;
        public float targetXRight;
        public float time;

        public void OpenDoor()
        {
            AudioController.instance.PlaySoundNVibrate(AudioController.instance.openDoor, 0);
            leftDoor.transform.DOLocalMoveX(targetXLeft, time).SetEase(Ease.Linear);
            rightDoor.transform.DOLocalMoveX(targetXRight, time).SetEase(Ease.Linear);
        }

        public IEnumerator ElevatorMoveUp(List<Player> poppies)
        {
            UIController.instance.layerCover.raycastTarget = true;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 10, transform.localPosition.z);
            Transform playerController = PlayerController.instance.transform;
            playerController.DOKill();
            float startY = playerController.localPosition.y;
            PlayerController.instance.handTutorial.Hide();
            PlayerController.instance.playerTouchMovement.navMeshAgent.enabled = false;
            playerController.localPosition = new Vector3(playerController.localPosition.x, playerController.localPosition.y - 10, playerController.localPosition.z);
            for (int i = 0; i < poppies.Count; i++)
            {
                poppies[i].DOKill();
                poppies[i].navMeshAgent.enabled = false;
                poppies[i].transform.localPosition = new Vector3(poppies[i].transform.localPosition.x, poppies[i].transform.localPosition.y - 10, poppies[i].transform.localPosition.z);
            }
            yield return new WaitForSeconds(0.5f);
            AudioController.instance.PlayElevator();
            for (int i = 0; i < poppies.Count; i++)
            {
                poppies[i].transform.DOLocalMoveY(startY, 1f);
            }
            playerController.DOLocalMoveY(startY, 1f).OnComplete(delegate
            {
                PlayerController.instance.playerTouchMovement.navMeshAgent.enabled = true;
                for (int i = 0; i < poppies.Count; i++)
                {
                    poppies[i].navMeshAgent.enabled = true;
                }
                AudioController.instance.StopElevator();
                OpenDoor();
                if (GameController.instance.IsBoss()) StartCoroutine(UIController.instance.BossIntro());
                else
                {
                    PlayerController.instance.handTutorial.PlayHand();
                    //UIManager.instance.ShowUIHome();
                    DOVirtual.DelayedCall(0.5f, delegate
                    {
                        UIController.instance.layerCover.raycastTarget = false;
                    });
                }
            });
            transform.DOLocalMoveY(0, 1f);
        }
    }
}
