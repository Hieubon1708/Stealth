using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hunter
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData")]
    public class LevelDataSO : ScriptableObject
    {
        public List<GameManager.StageType> levelTypes = new List<GameManager.StageType>();
    }
}