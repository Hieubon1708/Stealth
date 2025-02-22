using ACEPlay.Bridge;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Hunter
{
    public class GamePlay : MonoBehaviour
    {
        public TextMeshProUGUI textRemainingEnemy;
        public CanvasGroup remainingEnemy;
        public GameObject iconComplete;
        public GameObject panelLose;
        public GameObject panelWin;
        public GameObject frameRemainingEnemy;

        public void Play()
        {
            iconComplete.SetActive(false);
            remainingEnemy.DOFade(1f, 0.5f).SetEase(Ease.Linear);
            PlayerController.instance.handTutorial.StopHand();
        }

        public void UpdateRemainingEnemy()
        {
            if (GameController.instance.bots.Count > 0) textRemainingEnemy.text = GameController.instance.bots.Count.ToString();
            else
            {
                textRemainingEnemy.text = "";
                iconComplete.SetActive(true);
            }
            textRemainingEnemy.DOKill();
            textRemainingEnemy.transform.DOScale(1.25f, 0.1f).OnComplete(delegate
            {
                textRemainingEnemy.transform.DOScale(1f, 0.25f);
            });
        }

        public void Win()
        {
            /*for (int i = 0; i < GameController.instance.poppies.Count; i++)
            {
                if (GameController.instance.tempWeaponOnGround.ContainsKey(GameController.instance.poppies[i].gameObject))
                {
                    GameController.instance.tempWeaponPoppies[i] = GameController.instance.tempWeaponOnGround[GameController.instance.poppies[i].gameObject];
                }
            }*/
        }

        public void Lose()
        {
            //EventManager.EmitEvent(EventVariables.StopCountDownShowAdsInGame);
            BridgeController.instance.LogLevelFailWithParameter("stealk", GameManager.instance.Level);
        }

        public void LoseProgress()
        {
            AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);
        }

        public void Revival()
        {
            AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);
            UnityEvent e = new UnityEvent();
            e.AddListener(() =>
            {
                /*GameManager.instance.IsStart = true;
                Manager.instance.Stage = tempStage;
                Manager.instance.RescuedCharacter = tempPoppies;
                GameManager.instance.WeaponCharacter = tempWeaponPoppies;
                FadeManager.instance.FadeIn(delegate
                {
                    GameController.instance.LoadLevel(GameManager.instance.Level);
                    FadeManager.instance.FadeOut();
                });*/
            });
            BridgeController.instance.ShowRewarded("revival", e);
        }

        public void UpdateCoin(int coin)
        {
            if (coin == 0) return;
            /*EventManager.SetDataGroup(EventVariables.UpdateMission, MissionType.CollectMoney, coin);
            EventManager.EmitEvent(EventVariables.UpdateMission);*/
            BridgeController.instance.LogEarnCurrency("money", coin, "kill_and_ground_stealk");
            PlayerController.instance.takeMoney.TakeOn(coin);
            AudioController.instance.PlaySoundNVibrate(AudioController.instance.coin, 0);
            //UIEconomy.instance.AddCash(coin);
        }

        public void ChangeWeapon(int index)
        {
            
        }

        public void LoadUI()
        {
            panelLose.SetActive(false);
            remainingEnemy.alpha = 0;
            UpdateRemainingEnemy();
        }
    }
}
