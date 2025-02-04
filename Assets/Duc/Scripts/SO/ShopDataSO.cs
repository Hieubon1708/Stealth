using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duc
{
    [CreateAssetMenu(fileName = "ShopData", menuName = "ScriptableObjects/ShopData")]
    public class ShopDataSO : ScriptableObject
    {
        public List<ShopPack> shopPacks = new List<ShopPack>();
    }

    [System.Serializable]
    public class ShopPack
    {
        public string Name;
        public string keyIAP;
        public float price;
        public bool isNonConsumable;
        public string ribbonDescription;
        public Sprite sprIcon;
        public List<Reward> rewards;
    }
}