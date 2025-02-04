using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hunter
{
    public class PanelLose : MonoBehaviour
    {
        public void OnEnable()
        {
            ACEPlay.Native.NativeAds.instance.DisplayNativeAds(true);
            ACEPlay.Bridge.BridgeController.instance.ShowBannerCollapsible();
            AudioController.instance.PlaySoundNVibrate(AudioController.instance.lose, 0);
        }

        public void OnDisable()
        {
            ACEPlay.Native.NativeAds.instance.DisplayNativeAds(false);
            ACEPlay.Bridge.BridgeController.instance.HideBannerCollapsible();
        }
    }
}
