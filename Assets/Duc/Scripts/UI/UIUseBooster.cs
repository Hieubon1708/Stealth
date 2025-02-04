using Duc.PoppyTangle;
using System.Collections;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;

namespace Duc
{
    public class UIUseBooster : UI
    {
        [SerializeField] private GameObject axePanel;
        [SerializeField] private GameObject hammerPanel;
        [SerializeField] private GameObject freezeTimePanel;

        public override void Start()
        {
            base.Start();
        }

        public override void Show()
        {
            base.Show();

            Item booster = (Item)EventManager.GetInt(EventVariables.UseBooster);
            axePanel.SetActive(booster == Item.Axe);
            hammerPanel.SetActive(booster == Item.Hammer);
            freezeTimePanel.SetActive(booster == Item.FreezeTime);

            GameplayController.instance.gameplayStatus = GameplayStatus.Pause;
        }

        public void OnPointerUp()
        {
            Item booster = (Item)EventManager.GetInt(EventVariables.UseBooster);
            if (booster != Item.Axe && booster != Item.Hammer) return;

            switch (booster)
            {
                case Item.Axe:
                    EventManager.EmitEventData(EventVariables.CutRope, UIManager.instance.PositionTouch());
                    break;
                case Item.Hammer:
                    EventManager.EmitEventData(EventVariables.BreakLockedPin, UIManager.instance.PositionTouch());
                    break;
            }
        }

        public void OnClickButtonFreezeTime()
        {
            int freeze = Manager.instance.FreezeTime;
            Manager.instance.FreezeTime = freeze - 1;
            EventManager.EmitEvent(EventVariables.Freeze);
            UIManager.instance.UIInGame.DisplayButtonFreeze();

            EventManager.SetDataGroup(EventVariables.UpdateMission, MissionType.UseBooster, 1);
            EventManager.EmitEvent(EventVariables.UpdateMission);

            Hide();
        }

        public void OnClickButtonCancel()
        {
            Hide();
        }

        public override void Hide()
        {
            UIManager.instance.UIInGame.DisplayButtonAxe();
            UIManager.instance.UIInGame.DisplayButtonHammer();
            UIManager.instance.UIInGame.DisplayButtonFreeze();

            GameplayController.instance.gameplayStatus = GameplayStatus.Playing;

            base.Hide();
        }
    }
}