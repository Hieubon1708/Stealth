using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duc.PoppyTangle
{
    [System.Serializable]
    public class LevelData
    {
        public float duration;
        public float fieldOfView;
        public string wallId;
        public List<SlotElementData> slotElementDatas = new List<SlotElementData>();
        public List<PinElementData> pinElementDatas = new List<PinElementData>();
        public List<RopeElementData> ropeElementsDatas = new List<RopeElementData>();
    }

    [System.Serializable]
    public class RopeElementData
    {
        public int startPinId;
        public int endPinId;
        public float ropeLength;
        public float resolution;
        public List<PositionData> particlePositionsData = new List<PositionData>();
    }

    [System.Serializable]
    public class PinElementData
    {
        public int pinId;
        public bool isLocked;
        public int connectedSlotId;

        public PinElementData()
        {
        }

        public PinElementData(int pinId, bool isLocked, int connectedSlotId)
        {
            this.pinId = pinId;
            this.isLocked = isLocked;
            this.connectedSlotId = connectedSlotId;
        }
    }
    
    [System.Serializable]
    public class SlotElementData
    {
        public int slotId;
        public bool isLocked;
        public PositionData positionData;

        public SlotElementData()
        {
        }

        public SlotElementData(int slotId, bool isLocked, PositionData positionData)
        {
            this.slotId = slotId;
            this.isLocked = isLocked;
            this.positionData = positionData;
        }
    }

    public enum MaterialType
    {
        Red = 0,
        Blue = 1,
        Green = 2,
        Yellow = 3,
        Pink = 4,
        Purple = 5,
        Orange = 6,
        Gray = 7
    }

    [System.Serializable]
    public class PositionData
    {
        public float x;
        public float y;
        public float z;

        public PositionData()
        {
        }

        public PositionData(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }
        
        public PositionData(Vector3 position)
        {
            x = position.x;
            y = position.y;
            z = position.z;
        }
    }

    public static class Vector3Extensions
    {
        public static Vector3 ToVector3(this PositionData positionData)
        {
            return new Vector3(positionData.x, positionData.y, positionData.z);
        }
    }

}