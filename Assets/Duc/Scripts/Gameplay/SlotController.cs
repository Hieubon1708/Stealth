using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duc.PoppyTangle
{
    public class SlotController : MonoBehaviour
    {
        public int id;
        public bool isLocked = false;
        public PinController connectedPin;

        public GameObject objSlotNormal;
        public GameObject objSlotGlass;

        public GameObject objLock;
        public GameObject oldPlace;
        public GameObject highlight;

        private bool isPlay = false;

        public void SetGlass()
        {
            objSlotNormal.SetActive(false);
            objSlotGlass.SetActive(true);
            objLock.transform.localScale = Vector3.one * 0.75f;
        }

        public void SetLock(bool _isLocked)
        {
            isLocked = _isLocked;
            objLock.SetActive(isLocked);
        }

        public void PlayHighlight()
        {
            if (!isPlay)
            {
                isPlay = true;
                highlight.SetActive(true);
            }
        }

        public void PlayOldPlace()
        {
            if (!isPlay)
            {
                isPlay = true;
                oldPlace.SetActive(true);
            }
        }

        public void PlayEmpty()
        {
            isPlay = false;
            highlight.SetActive(false);
            oldPlace.SetActive(false);
        }
    }
}