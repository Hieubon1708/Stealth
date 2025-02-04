using Newtonsoft.Json;
using Obi;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.Terrain;

namespace Duc.PoppyTangle
{
    public class LevelGenerator : MonoBehaviour
    {
        public static LevelGenerator instance;

        public int level = 1;
        public float duration = 0f;
        public string wallId;

        public PinController selectedPin;
        private PinController cachePin;

        private void Awake()
        {
            if (instance == null) instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.L) && Input.GetMouseButtonDown(0))
            {
                Ray ray = GameplayController.instance.cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, GameplayController.instance.layerMask))
                {
                    if (hit.transform.TryGetComponent(out SlotController slot))
                    {
                        slot.SetLock(!slot.isLocked);
                    }
                    else if (hit.transform.TryGetComponent(out PinController pin))
                    {
                        pin.SetLock(!pin.isLocked);
                    }
                }

                return;
            }

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetMouseButtonDown(0))
            {
                Ray ray = GameplayController.instance.cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, GameplayController.instance.layerMask) && hit.transform.TryGetComponent(out SlotController slot))
                {
                    if (slot.connectedPin == null)
                    {
                        PinController newPin = Instantiate(Resources.Load<PinController>(Path.Combine("Gameplay", "Pin")), GameplayController.instance.pinParent);
                        newPin.transform.position = slot.transform.position;
                        slot.connectedPin = newPin;
                        newPin.connectedSlot = slot;

                        if (cachePin == null)
                        {
                            cachePin = newPin;
                        }
                        else
                        {
                            RopeController newRope = Instantiate(Resources.Load<RopeController>(Path.Combine("Gameplay", "Chain")), GameplayController.instance.ropeParent);

                            var blueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();
                            int filter = ObiUtils.MakeFilter(1 << 0, 0);
                            int filterEverything = ObiUtils.MakeFilter(ObiUtils.CollideWithEverything, 0);
                            blueprint.thickness = 0.1f;
                            blueprint.resolution = 0.5f;
                            blueprint.path.Clear();

                            blueprint.path.AddControlPoint(cachePin.ropeConnectPoint.position, Vector3.zero, Vector3.zero, Vector3.up, 0.1f, 0.1f, 1, filter, Color.white, "start");
                            //blueprint.path.AddControlPoint(cachePin.ropeConnectPoint.position - Vector3.forward * 0.05f, Vector3.zero, Vector3.zero, Vector3.up, 0.1f, 0.1f, 1, filter, Color.white, "start2");

                            //blueprint.path.AddControlPoint(((cachePin.ropeConnectPoint.position + newPin.ropeConnectPoint.position) / 2) + Vector3.back, Vector3.zero, Vector3.zero, Vector3.up, 0.1f, 0.1f, 1, filterEverything, Color.white, "mid_point");

                            //blueprint.path.AddControlPoint(newPin.ropeConnectPoint.position - Vector3.forward * 0.05f, Vector3.zero, Vector3.zero, Vector3.up, 0.1f, 0.1f, 1, filter, Color.white, "end2");

                            blueprint.path.AddControlPoint(newPin.ropeConnectPoint.position, Vector3.zero, Vector3.zero, Vector3.up, 0.1f, 0.1f, 1, filter, Color.white, "end");

                            blueprint.path.FlushEvents();
                            blueprint.GenerateImmediate();

                            newRope.obiRope.ropeBlueprint = blueprint;

                            cachePin.connectedRope = newRope;
                            newPin.connectedRope = newRope;

                            newRope.startPin = cachePin;
                            newRope.endPin = newPin;

                            var attachmentStart = newRope.gameObject.AddComponent<ObiParticleAttachment>();
                            //var attachmentStart2 = newRope.gameObject.AddComponent<ObiParticleAttachment>();
                            var attachmentEnd = newRope.gameObject.AddComponent<ObiParticleAttachment>();
                            //var attachmentEnd2 = newRope.gameObject.AddComponent<ObiParticleAttachment>();

                            attachmentStart.target = cachePin.ropeConnectPoint;
                            attachmentStart.particleGroup = blueprint.groups[0];

                            //attachmentStart2.target = cachePin.ropeConnectPoint;
                            //attachmentStart2.particleGroup = blueprint.groups[1];

                            //attachmentEnd2.target = newPin.ropeConnectPoint;
                            //attachmentEnd2.particleGroup = blueprint.groups[blueprint.groups.Count - 2];

                            attachmentEnd.target = newPin.ropeConnectPoint;
                            attachmentEnd.particleGroup = blueprint.groups[blueprint.groups.Count - 1];

                            GameplayController.instance.pins.Add(cachePin);
                            GameplayController.instance.pins.Add(newPin);

                            GameplayController.instance.ropes.Add(newRope);

                            MaterialType[] materialTypes = (MaterialType[])System.Enum.GetValues(typeof(MaterialType));
                            int materialIndex = 0;
                            foreach (var ropeTemp in GameplayController.instance.ropes)
                            {
                                ropeTemp.SetColor(materialTypes[materialIndex]);
                                ropeTemp.startPin.SetColor(materialTypes[materialIndex]);
                                ropeTemp.endPin.SetColor(materialTypes[materialIndex]);

                                materialIndex++;
                                if (materialIndex >= materialTypes.Length)
                                {
                                    materialIndex = 0;
                                }
                            }

                            cachePin = null;
                        }
                    }
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Ray ray = GameplayController.instance.cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, GameplayController.instance.layerMask) && hit.transform.TryGetComponent(out PinController pin))
                {
                    selectedPin = pin;
                    selectedPin.MouseDown();
                }
            }

            if (selectedPin != null)
            {
                Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, GameplayController.instance.cam.WorldToScreenPoint(selectedPin.transform.position).z);

                Vector3 worldPos = GameplayController.instance.cam.ScreenToWorldPoint(mousePos);
                selectedPin.Move(worldPos);

                if (Input.GetKey(KeyCode.UpArrow))
                {
                    selectedPin.connectedRope.ChangeLength(true);
                }

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    selectedPin.connectedRope.ChangeLength(false);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (selectedPin != null)
                {
                    selectedPin.MouseUp();
                    selectedPin = null;
                }
            }

            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                if (cachePin != null)
                {
                    Destroy(cachePin.gameObject);
                    cachePin = null;
                }
            }

            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
        }

        public void Load()
        {
            foreach (var slot in GameplayController.instance.slots)
            {
                Destroy(slot.gameObject);
            }

            if (GameplayController.instance.wall != null)
            {
                Destroy(GameplayController.instance.wall.gameObject);
            }

            GameplayController.instance.slots.Clear();

            TextAsset textAsset = Resources.Load<TextAsset>(Path.Combine("Data", $"Level_{level}"));

            if (textAsset == null)
            {
                Debug.LogError($"Level {level} not exist!!");
                return;
            }

            LevelData levelData = JsonConvert.DeserializeObject<LevelData>(textAsset.text);

            duration = levelData.duration;
            GameplayController.instance.cam.fieldOfView = levelData.fieldOfView;
            wallId = levelData.wallId;

            //GameplayController.instance.wall = Instantiate(Resources.Load<Wall>(Path.Combine("Walls", levelData.wallId)));
            GameplayController.instance.wall = Instantiate(Resources.Load<Wall>(Path.Combine("Walls", "Wall_1")));

            foreach (var slotData in levelData.slotElementDatas)
            {
                SlotController newSlot = Instantiate(Resources.Load<SlotController>(Path.Combine("Gameplay", "Slot")), GameplayController.instance.slotParent);
                newSlot.transform.position = slotData.positionData.ToVector3();
                newSlot.id = slotData.slotId;
                newSlot.SetLock(slotData.isLocked);

                GameplayController.instance.slots.Add(newSlot);
            }

            foreach (var pinData in levelData.pinElementDatas)
            {
                PinController newPin = Instantiate(Resources.Load<PinController>(Path.Combine("Gameplay", "Pin")), GameplayController.instance.pinParent);
                newPin.id = pinData.pinId;
                newPin.SetLock(pinData.isLocked);

                foreach (var slot in GameplayController.instance.slots)
                {
                    if (slot.id == pinData.connectedSlotId)
                    {
                        newPin.connectedSlot = slot;
                        slot.connectedPin = newPin;
                        newPin.transform.position = slot.transform.position;
                        break;
                    }
                }

                GameplayController.instance.pins.Add(newPin);
            }

            MaterialType[] materialTypes = (MaterialType[])System.Enum.GetValues(typeof(MaterialType));
            int materialIndex = 0;

            foreach (var ropeData in levelData.ropeElementsDatas)
            {
                RopeController newRope = Instantiate(Resources.Load<RopeController>(Path.Combine("Gameplay", "Chain")), GameplayController.instance.ropeParent);

                foreach (var pin in GameplayController.instance.pins)
                {
                    if (pin.id == ropeData.startPinId)
                    {
                        newRope.startPin = pin;
                        pin.connectedRope = newRope;
                    }

                    if (pin.id == ropeData.endPinId)
                    {
                        newRope.endPin = pin;
                        pin.connectedRope = newRope;
                    }

                    if (newRope.startPin != null && newRope.endPin != null) break;
                }

                var blueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();
                int filter = ObiUtils.MakeFilter(1 << 0, 0);
                int filterEverything = ObiUtils.MakeFilter(ObiUtils.CollideWithEverything, 0);
                blueprint.thickness = 0.1f;
                blueprint.resolution = (ropeData.particlePositionsData.Count / (ropeData.ropeLength / 0.1f)) / 10f;
                blueprint.path.Clear();

                blueprint.path.AddControlPoint(ropeData.particlePositionsData[0].ToVector3(), Vector3.zero, Vector3.zero, Vector3.up, 0.1f, 0.1f, 1, filter, Color.white, "start");
                blueprint.path.AddControlPoint(ropeData.particlePositionsData[1].ToVector3(), Vector3.zero, Vector3.zero, Vector3.up, 0.1f, 0.1f, 1, filter, Color.white, "start2");

                for (int i = 2; i < ropeData.particlePositionsData.Count - 2; i++)
                {
                    blueprint.path.AddControlPoint(ropeData.particlePositionsData[i].ToVector3(), Vector3.zero, Vector3.zero, Vector3.up, 0.1f, 0.1f, 1, filterEverything, Color.white, "mid_point");
                }

                blueprint.path.AddControlPoint(ropeData.particlePositionsData[ropeData.particlePositionsData.Count - 2].ToVector3(), Vector3.zero, Vector3.zero, Vector3.up, 0.1f, 0.1f, 1, filter, Color.white, "end2");
                blueprint.path.AddControlPoint(ropeData.particlePositionsData[ropeData.particlePositionsData.Count - 1].ToVector3(), Vector3.zero, Vector3.zero, Vector3.up, 0.1f, 0.1f, 1, filter, Color.white, "end");

                blueprint.path.FlushEvents();
                blueprint.GenerateImmediate();

                newRope.obiRope.ropeBlueprint = blueprint;

                var attachmentStart = newRope.gameObject.AddComponent<ObiParticleAttachment>();
                var attachmentStart2 = newRope.gameObject.AddComponent<ObiParticleAttachment>();
                var attachmentEnd = newRope.gameObject.AddComponent<ObiParticleAttachment>();
                var attachmentEnd2 = newRope.gameObject.AddComponent<ObiParticleAttachment>();

                attachmentStart.target = newRope.startPin.ropeConnectPoint;
                attachmentStart.particleGroup = blueprint.groups[0];

                //attachmentStart2.target = newRope.startPin.ropeConnectPoint;
                //attachmentStart2.particleGroup = blueprint.groups[1];

                //attachmentEnd2.target = newRope.endPin.ropeConnectPoint;
                //attachmentEnd2.particleGroup = blueprint.groups[blueprint.groups.Count - 2];

                attachmentEnd.target = newRope.endPin.ropeConnectPoint;
                attachmentEnd.particleGroup = blueprint.groups[blueprint.groups.Count - 1];

                newRope.SetColor(materialTypes[materialIndex]);
                newRope.startPin.SetColor(materialTypes[materialIndex]);
                newRope.endPin.SetColor(materialTypes[materialIndex]);

                materialIndex++;
                if (materialIndex >= materialTypes.Length)
                {
                    materialIndex = 0;
                }

                GameplayController.instance.ropes.Add(newRope);
            }

            Debug.Log($"Load level {level} success!!");
        }

        public void Save()
        {
            LevelData levelData = new LevelData();

            levelData.duration = duration;
            levelData.fieldOfView = GameplayController.instance.cam.fieldOfView;
            levelData.wallId = wallId;

            int slotId = 0;

            if (GameplayController.instance.slotParent.childCount > 0)
            {
                foreach (Transform child in GameplayController.instance.slotParent)
                {
                    if (child.gameObject.activeSelf && child.TryGetComponent(out SlotController slot))
                    {
                        slot.id = slotId;

                        SlotElementData newSlotElementData = new SlotElementData(slotId, slot.isLocked, new PositionData(child.transform.position));
                        levelData.slotElementDatas.Add(newSlotElementData);

                        slotId++;
                    }
                }
            }
            else
            {
                foreach (var slot in GameplayController.instance.slots)
                {
                    slot.id = slotId;

                    SlotElementData newSlotElementData = new SlotElementData(slotId, slot.isLocked, new PositionData(slot.transform.position));
                    levelData.slotElementDatas.Add(newSlotElementData);

                    slotId++;
                }
            }

            int pinId = 0;
            foreach (Transform child in GameplayController.instance.pinParent)
            {
                if (child.TryGetComponent(out PinController pin))
                {
                    pin.id = pinId;

                    PinElementData newPinElementData = new PinElementData(pinId, pin.isLocked, pin.connectedSlot.id);
                    levelData.pinElementDatas.Add(newPinElementData);

                    pinId++;
                }
            }

            foreach (Transform child in GameplayController.instance.ropeParent)
            {
                if (child.TryGetComponent(out RopeController rope))
                {
                    RopeElementData newRopeElementData = new RopeElementData();
                    newRopeElementData.startPinId = rope.startPin.id;
                    newRopeElementData.endPinId = rope.endPin.id;
                    newRopeElementData.ropeLength = rope.obiRope.restLength;
                    newRopeElementData.resolution = (rope.obiRope.elements.Count + 1) / (rope.obiRope.restLength / 0.1f);

                    foreach (var element in rope.obiRope.elements)
                    {
                        PositionData positionData = new PositionData(rope.obiRope.solver.positions[element.particle1]);
                        newRopeElementData.particlePositionsData.Add(positionData);
                    }

                    PositionData lastPositionData = new PositionData(rope.obiRope.solver.positions[rope.obiRope.elements[rope.obiRope.elements.Count - 1].particle2]);
                    newRopeElementData.particlePositionsData.Add(lastPositionData);

                    levelData.ropeElementsDatas.Add(newRopeElementData);
                }
            }

            string txt = JsonConvert.SerializeObject(levelData);
            string path = Path.Combine(Application.dataPath, "Duc", "Resources", "Data");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, string.Concat(string.Format("Level_{0}", level), ".json"));
            File.WriteAllText(path, txt);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif

            Debug.Log($"Save level {level} success!!!");

        }

    }
}