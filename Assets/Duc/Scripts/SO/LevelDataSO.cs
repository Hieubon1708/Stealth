using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duc
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData")]
    public class LevelDataSO : ScriptableObject
    {
        public List<Chapter> chapters = new List<Chapter>();
    }

    [System.Serializable]
    public class Chapter
    {
        public List<StageType> stages = new List<StageType>();
    }
}