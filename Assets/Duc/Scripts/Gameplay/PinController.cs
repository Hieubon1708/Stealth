using DG.Tweening;
using System.IO;
using UnityEngine;

namespace Duc.PoppyTangle
{
    public class PinController : MonoBehaviour
    {
        public int id;
        public bool isLocked = false;
        public bool isMoving = false;

        public Transform ropeConnectPoint;

        public SlotController connectedSlot;
        public SlotController nearestSlot;

        public RopeController connectedRope;

        public GameObject model;
        public GameObject objLock;

        public Renderer meshRenderer;
        public MaterialType materialType;

//#if UNITY_EDITOR
//        private void OnValidate()
//        {
//            if (objLock != null)
//            {
//                objLock.SetActive(isLocked);
//            }
//        }
//#endif

        public void SetColor(MaterialType _materialType)
        {
            materialType = _materialType;
            Material colorMat = Resources.Load<Material>(Path.Combine("Materials", "Pin", _materialType.ToString()));
            meshRenderer.material = colorMat;
        }

        public void SetLock(bool _isLocked)
        {
            isLocked = _isLocked;
            objLock.SetActive(isLocked);
        }

        public void MouseDown()
        {
            isMoving = true;
            transform.DOKill();

            connectedSlot.PlayOldPlace();

            if (Manager.instance.LevelPoppyTangle == 1)
            {
                UIManager.instance.UIInGame.hand.DOKill();
                UIManager.instance.UIInGame.hand.gameObject.SetActive(false);
            }

            AudioController.instance.PlaySoundPickUp();
        }

        public void MouseUp()
        {
            if (nearestSlot != null && nearestSlot != connectedSlot && nearestSlot.connectedPin == null)
            {
                connectedSlot.PlayEmpty();
                nearestSlot.PlayEmpty();

                transform.DOMove(nearestSlot.transform.position, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    if (connectedRope.IsRopeMaxLength(true))
                    {
                        transform.DOMove(connectedSlot.transform.position, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            if (nearestSlot != null)
                            {
                                nearestSlot.PlayEmpty();
                                nearestSlot = null;
                            }
                            connectedSlot.PlayEmpty();

                            isMoving = false;
                            GameplayController.instance.SetCanMerge(true);

                            GameplayController.instance.CheckTutorial();
                        });
                    }
                    else
                    {
                        connectedSlot.connectedPin = null;
                        connectedSlot = nearestSlot;
                        connectedSlot.connectedPin = this;
                        nearestSlot = null;

                        isMoving = false;
                        GameplayController.instance.SetCanMerge(true);

                        if (Manager.instance.LevelPoppyTangle == 1 && connectedSlot != GameplayController.instance.slots[0])
                        {
                            GameplayController.instance.CheckTutorial();
                        }
                    }
                });
            }
            else
            {
                transform.DOMove(connectedSlot.transform.position, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    if (nearestSlot != null)
                    {
                        nearestSlot.PlayEmpty();
                        nearestSlot = null;
                    }
                    connectedSlot.PlayEmpty();

                    isMoving = false;
                    GameplayController.instance.SetCanMerge(true);

                    GameplayController.instance.CheckTutorial();
                });
            }

            AudioController.instance.PlaySoundDrop();
        }

        public void Move(Vector3 pos)
        {
            Vector2 to = pos;

            float maxDistance = 3.5f;

            PinController otherPin;
            if (connectedRope.startPin != this)
            {
                otherPin = connectedRope.startPin;
            }
            else
            {
                otherPin = connectedRope.endPin;
            }

            float targetDistance = Vector2.Distance(pos, otherPin.transform.position);
            if (targetDistance > maxDistance)
            {
                float diffDistance = targetDistance - maxDistance;
                Vector2 dir = Vector3.Normalize((Vector2)otherPin.transform.position - to);
                to += dir * diffDistance;
            }

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(to.x, to.y, -0.75f), 20 * Time.deltaTime);
            //transform.position = new Vector3(to.x, to.y, transform.position.z);

            nearestSlot = GameplayController.instance.GetNearestSlot(this);

            if (nearestSlot.connectedPin == null)
            {
                nearestSlot.PlayHighlight();
                foreach (var slot in GameplayController.instance.slots)
                {
                    if (slot != nearestSlot && slot != connectedSlot)
                    {
                        slot.PlayEmpty();
                    }
                }
            }
            else
            {
                foreach (var slot in GameplayController.instance.slots)
                {
                    if (slot != connectedSlot)
                    {
                        slot.PlayEmpty();
                    }
                }
            }

            if (connectedRope.IsRopeMaxLength(false))
            {
                if (nearestSlot != null)
                {
                    nearestSlot.PlayEmpty();
                    nearestSlot = null;
                }

                GameplayController.instance.selectedPin = null;
                MouseUp();
            }
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }
    }
}