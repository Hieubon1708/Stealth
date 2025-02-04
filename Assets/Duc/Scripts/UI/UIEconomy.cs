using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TigerForge;
using TMPro;
using UnityEngine;

namespace Duc
{
    public class UIEconomy : MonoBehaviour
    {
        public static UIEconomy instance;

        [SerializeField] private RectTransform rectParent;

        [Header("Cash")]
        public GameObject cashContainer;
        [SerializeField] private TextMeshProUGUI txtTotalCash;
        [SerializeField] private Transform iconCash;
        [SerializeField] private GameObject cashUIPrefab;

        [Header("Gem")]
        [SerializeField] private TextMeshProUGUI txtTotalGem;
        [SerializeField] private Transform iconGem;
        [SerializeField] private GameObject gemUIPrefab;

        [Header("Button")]
        [SerializeField] private GameObject[] btnsRemoveAds;

        private void Awake()
        {
            if (instance == null) instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            DisplayTotalCash();
            DisplayTotalGem();
        }

//#if UNITY_EDITOR
//        private void Update()
//        {
//            if (Input.GetKeyDown(KeyCode.C))
//            {
//                AddCash(100, transform);
//            }

//            if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
//            {
//                AddCash(100, Input.mousePosition);
//            }

//            if (Input.GetKeyDown(KeyCode.G))
//            {
//                AddGem(100, transform);
//            }

//            if (Input.GetKey(KeyCode.RightControl) && Input.GetMouseButtonDown(0))
//            {
//                AddGem(100, Input.mousePosition);
//            }
//        }
//#endif

        public void AddCash(int amount, Transform from = null)
        {
            int cash = Manager.instance.Money;
            Manager.instance.Money = cash + amount;
            Manager.instance.TotalEarn += amount;

            EventManager.SetDataGroup(EventVariables.UpdateMission, MissionType.CollectMoney, amount);
            EventManager.EmitEvent(EventVariables.UpdateMission);

            if (from != null)
            {
                for (int i = 0; i < 10; i++)
                {
                    GameObject obj = cashUIPrefab.Spawn(transform);

                    var posInstantiate = UnityEngine.Random.insideUnitSphere * 58f;
                    posInstantiate.z = from.position.z;
                    posInstantiate += from.position;

                    obj.transform.position = posInstantiate;

                    obj.SetActive(false);

                    obj.transform.DOMove(iconCash.position, 1f).SetEase(Ease.InBack).SetDelay(i * 0.05f)
                    .OnStart(() =>
                    {
                        obj.SetActive(true);
                    }).OnComplete(() =>
                    {
                        obj.GetComponentInChildren<ParticleSystem>().Play();

                        DOVirtual.DelayedCall(1f, () =>
                        {
                            obj.Recycle();
                        });

                        AudioController.instance.PlaySoundCoinFly();
                    }).SetUpdate(true);
                }

                DisplayTotalCash(true, cash, 1f);
            }
            else
            {
                DisplayTotalCash(true, cash, 0f);
            }

            ACEPlay.Bridge.BridgeController.instance.SetPropertyCoinEarn(Manager.instance.TotalEarn.ToString());
        }

        public void AddCash(int amount, Vector2 screenPoint)
        {
            int cash = Manager.instance.Money;
            Manager.instance.Money = cash + amount;
            Manager.instance.TotalEarn += amount;

            EventManager.SetDataGroup(EventVariables.UpdateMission, MissionType.CollectMoney, amount);
            EventManager.EmitEvent(EventVariables.UpdateMission);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPoint, null, out Vector2 anchorPos);

            for (int i = 0; i < Mathf.Min(amount, 10); i++)
            {
                GameObject obj = cashUIPrefab.Spawn(transform);

                Vector2 posInstantiate = UnityEngine.Random.insideUnitSphere * 58f;
                posInstantiate += anchorPos;

                obj.transform.localPosition = new Vector3(posInstantiate.x, posInstantiate.y, 0);

                obj.SetActive(false);

                obj.transform.DOMove(iconCash.position, 1f).SetEase(Ease.InBack).SetDelay(i * 0.05f)
                .OnStart(() =>
                {
                    obj.SetActive(true);
                }).OnComplete(() =>
                {
                    obj.GetComponentInChildren<ParticleSystem>().Play();

                    DOVirtual.DelayedCall(1f, () =>
                    {
                        obj.Recycle();
                    });

                    AudioController.instance.PlaySoundCoinFly();
                }).SetUpdate(true);
            }

            DisplayTotalCash(true, cash, 1f);

            ACEPlay.Bridge.BridgeController.instance.SetPropertyCoinEarn(Manager.instance.TotalEarn.ToString());
        }

        public void AddGem(int amount, Transform from = null)
        {
            int gem = Manager.instance.Gem;
            Manager.instance.Gem = gem + amount;

            if (from != null)
            {
                for (int i = 0; i < 10; i++)
                {
                    GameObject obj = gemUIPrefab.Spawn(transform);

                    var posInstantiate = UnityEngine.Random.insideUnitSphere * 58f;
                    posInstantiate.z = from.position.z;
                    posInstantiate += from.position;

                    obj.transform.position = posInstantiate;

                    obj.SetActive(false);

                    obj.transform.DOMove(iconGem.position, 1f).SetEase(Ease.InBack).SetDelay(i * 0.05f)
                    .OnStart(() =>
                    {
                        obj.SetActive(true);
                    }).OnComplete(() =>
                    {
                        obj.GetComponentInChildren<ParticleSystem>().Play();

                        DOVirtual.DelayedCall(1f, () =>
                        {
                            obj.Recycle();
                        });

                        AudioController.instance.PlaySoundGemFly();
                    }).SetUpdate(true);
                }

                DisplayTotalGem(true, gem, 1f);
            }
            else
            {
                DisplayTotalGem(true, gem, 0f);
            }
        }

        public void AddGem(int amount, Vector2 screenPoint)
        {
            int gem = Manager.instance.Gem;
            Manager.instance.Gem = gem + amount;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPoint, null, out Vector2 anchorPos);

            for (int i = 0; i < Mathf.Min(amount, 10); i++)
            {
                GameObject obj = gemUIPrefab.Spawn(transform);

                Vector2 posInstantiate = UnityEngine.Random.insideUnitSphere * 58f;
                posInstantiate += anchorPos;

                obj.transform.localPosition = new Vector3(posInstantiate.x, posInstantiate.y, 0);

                obj.SetActive(false);

                obj.transform.DOMove(iconGem.position, 1f).SetEase(Ease.InBack).SetDelay(i * 0.05f)
                .OnStart(() =>
                {
                    obj.SetActive(true);
                }).OnComplete(() =>
                {
                    obj.GetComponentInChildren<ParticleSystem>().Play();

                    DOVirtual.DelayedCall(1f, () =>
                    {
                        obj.Recycle();
                    });

                    AudioController.instance.PlaySoundGemFly();
                }).SetUpdate(true);
            }

            DisplayTotalGem(true, gem, 1f);
        }

        public void DisplayTotalCash(bool isSmooth = false, int from = 0, float delay = 0f)
        {
            int cash = Manager.instance.Money;
            if (isSmooth)
            {
                DOVirtual.Int(from, cash, 1f, result =>
                {
                    txtTotalCash.text = result.ToString();
                }).SetDelay(delay).SetEase(Ease.Linear).SetUpdate(true);
            }
            else
            {
                txtTotalCash.text = cash.ToString();
            }
        }

        public void DisplayTotalGem(bool isSmooth = false, int from = 0, float delay = 0f)
        {
            int gem = Manager.instance.Gem;
            if (isSmooth)
            {
                DOVirtual.Int(from, gem, 1f, result =>
                {
                    txtTotalGem.text = result.ToString();
                }).SetDelay(delay).SetEase(Ease.Linear).SetUpdate(true);
            }
            else
            {
                txtTotalGem.text = gem.ToString();
            }
        }
    }
}