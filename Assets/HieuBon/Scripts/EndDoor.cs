using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Hunter
{
    public class EndDoor : MonoBehaviour
    {
        public GameObject leftDoor;
        public GameObject rightDoor;
        public float targetXLeft;
        public float targetXRight;
        public float time;
        public GameObject arrow;
        Coroutine openDoor;
        public BoxCollider col;

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player") && !GameController.instance.IsKilling()
                && !UIController.instance.layerCover.raycastTarget)
            {
                openDoor = StartCoroutine(Win(transform.position));
            }
        }

        public void StopDoor()
        {
            if (openDoor != null)
            {
                StopCoroutine(openDoor);
                openDoor = null;
            }
        }

        public IEnumerator Win(Vector3 endPointPosition)
        {
            UIController.instance.layerCover.raycastTarget = true;          
            PlayerController.instance.Win();
            GameController.instance.Win();
            PlayerController.instance.playerTouchMovement.navMeshAgent.destination = arrow.transform.position;
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitUntil(() => PlayerController.instance.playerTouchMovement.navMeshAgent.remainingDistance == PlayerController.instance.playerTouchMovement.navMeshAgent.stoppingDistance);
            AudioController.instance.PlaySoundNVibrate(AudioController.instance.openDoor, 0);
            leftDoor.transform.DOLocalMoveX(targetXLeft, time).SetEase(Ease.Linear);
            rightDoor.transform.DOLocalMoveX(targetXRight, time).SetEase(Ease.Linear);
            yield return new WaitForSeconds(time / 3);
            PlayerController.instance.playerTouchMovement.navMeshAgent.destination = endPointPosition;
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitUntil(() => PlayerController.instance.playerTouchMovement.navMeshAgent.remainingDistance == PlayerController.instance.playerTouchMovement.navMeshAgent.stoppingDistance);
            yield return new WaitForSeconds(time / 3);
            leftDoor.transform.DOLocalMoveX(0, time * 0.75f).SetEase(Ease.Linear);
            rightDoor.transform.DOLocalMoveX(0, time * 0.75f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(time);
            UIController.instance.Win();
            GameManager.instance.Win();
            for (int i = 0; i < GameController.instance.poppies.Count; i++)
            {
                GameController.instance.poppies[i].navMeshAgent.enabled = false;
                GameController.instance.poppies[i].transform.DOLocalMoveY(GameController.instance.poppies[i].transform.localPosition.y + 10, 1f);
            }
            PlayerController.instance.playerTouchMovement.navMeshAgent.enabled = false;
            PlayerController.instance.transform.DOMoveY(PlayerController.instance.transform.position.y + 10, 1f);
            UIController.instance.virtualCam.ElevatorMoveUp(1f);
            UIController.instance.ChangeMap();
        }
    }
}