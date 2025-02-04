using Duc.PoppyTangle;
using TMPro;
using UnityEngine;

namespace Duc
{
    public class UICheat : UI
    {
        [SerializeField] TMP_InputField inputLevel;
        [SerializeField] TMP_InputField inputPassword;

        public override void Show()
        {
            base.Show();

            gameObject.SetActive(true);
        }

        public override void Hide()
        {
            base.Hide();

            gameObject.SetActive(false);
        }

        public void OnClickButtonPlay()
        {
            if (inputPassword.text == "minhduc12")
            {
                if (!string.IsNullOrEmpty(inputLevel.text))
                {
                    Manager.instance.LevelPoppyTangle = int.Parse(inputLevel.text);

                    GameplayController.instance.Init();
                }

                Hide();
            }
        }

        public void OnClickButtonRemoveAds()
        {
            if (inputPassword.text == "minhduc12")
            {
                ACEPlay.Bridge.BridgeController.instance.CanShowAds = false;
            }
        }

        bool isShowingUI = true;
        public void OnClickButtonShowHideUI()
        {
            if (inputPassword.text == "minhduc12")
            {
                isShowingUI = !isShowingUI;

                UIManager.instance.UIInGame.SetActiveUI(isShowingUI);
                UIEconomy.instance.cashContainer.SetActive(isShowingUI);
            }
        }
        
        public void OnClickButtonAddCoin()
        {
            if (inputPassword.text == "minhduc12")
            {
                UIEconomy.instance.AddCash(1000);
            }
        }

        public void OnClickButtonClose()
        {
            Hide();
        }
    }
}