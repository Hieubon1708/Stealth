using ACEPlay.Bridge;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

namespace Hunter
{
    public class WeaponOnGround : MonoBehaviour
    {
        public CapsuleCollider col;
        public GameController.WeaponType weaponType;
        public GameObject button;
        public ParticleSystem bum;
        public GameObject parent;
        public GameObject buttonBuy;
        bool isSee;

        public void Watch()
        {
            AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);
            UnityEvent e = new UnityEvent();
            e.AddListener(() =>
            {
                bum.Play();
                parent.SetActive(false);
                GameController.instance.AddWeapon(weaponType);
                AudioController.instance.PlaySoundNVibrate(AudioController.instance.pickUp, 0);
            });
            BridgeController.instance.ShowRewarded("loot", e);
        }

        public void Buy()
        {
            int price = 300;
            if (Duc.Manager.instance.Money < price) return;
            Duc.Manager.instance.Money -= price;
            BridgeController.instance.LogSpendCurrency("money", price, weaponType.ToString() + "_loot");
            AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);
            AudioController.instance.PlaySoundNVibrate(AudioController.instance.pickUp, 0);
            Duc.UIEconomy.instance.DisplayTotalCash(true, price);
            bum.Play();
            parent.SetActive(false);
            GameController.instance.AddWeapon(weaponType);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                col.enabled = false;
                bum.Play();
                parent.SetActive(false);
                GameController.instance.AddWeapon(weaponType);
                AudioController.instance.PlaySoundNVibrate(AudioController.instance.pickUp, 0);
            }
        }

        private void LateUpdate()
        {
            if (button != null && parent.activeSelf)
            {
                button.transform.LookAt(new Vector3(transform.position.x, GameController.instance.cam.transform.position.y, GameController.instance.cam.transform.position.z));
                float distance = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
                if (distance <= 5)
                {
                    if (!isSee)
                    {
                        isSee = true;
                        button.SetActive(true);
                        buttonBuy.SetActive(Duc.Manager.instance.Money >= 300);
                    }
                }
                else if (isSee)
                {
                    isSee = false;
                    button.SetActive(false);
                }
            }
        }
    }
}
