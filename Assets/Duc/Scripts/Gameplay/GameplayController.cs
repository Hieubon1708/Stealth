using DG.Tweening;
using Newtonsoft.Json;
using Obi;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;

namespace Duc.PoppyTangle
{
    public class GameplayController : MonoBehaviour
    {
        public static GameplayController instance;

        public Camera cam;
        public LayerMask layerMask;

        public GameplayStatus gameplayStatus = GameplayStatus.None;
        public GameMode gameMode = GameMode.Tangle;

        public GameObject backgroundTangle;
        public GameObject backgroundDailyChallenge;

        public GameObject board;
        public GameObject game;
        public ObiSolver solver;

        public List<SlotController> slots = new List<SlotController>();
        public List<PinController> pins = new List<PinController>();
        public List<RopeController> ropes = new List<RopeController>();

        public Transform slotParent;
        public Transform pinParent;
        public Transform ropeParent;
        public Transform effectParent;

        public Wall wall;
        public GameObject wallTemp;

        public GameObject axePrefab;
        public GameObject hammerPrefab;
        public GameObject vfxMerge;

        public bool canMerge = true;

        public PinController selectedPin;

        public CharacterController characterController;
        public Transform characterParent;

        private void Awake()
        {
            if (instance == null) instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            if (transform.GetComponent<LevelGenerator>() != null) return;

            int chapter = Mathf.Min(Manager.instance.Chapter, Manager.instance.levelDataSO.chapters.Count);
            int stage = Manager.instance.Stage;

            Chapter currentChapter = Manager.instance.levelDataSO.chapters[chapter - 1];

            if (currentChapter.stages[stage - 1] == StageType.Tangle)
            {
                SetUp();
                AudioController.instance.PlayMenuMusic();
            }
            else
            {
                FadeManager.instance.LoadSceneOnStart(2, () =>
                {
                    QuitGame();
                });
            }

            EventManager.StartListening(EventVariables.CutRope, CutRope);
            EventManager.StartListening(EventVariables.BreakLockedPin, BreakLockedPin);
        }

        // Update is called once per frame
        void Update()
        {
            if (gameplayStatus == GameplayStatus.Playing)
            {
                if (Input.GetMouseButtonDown(0) && !UIManager.instance.IsPoiterUI())
                {
                    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
                    {
                        if (hit.transform.TryGetComponent(out PinController pin))
                        {
                            if (pin.isLocked) return;

                            selectedPin = pin;

                            if (selectedPin.connectedRope.isMerge)
                            {
                                selectedPin = null;
                                return;
                            }

                            selectedPin.MouseDown();

                            EventManager.EmitEvent(EventVariables.StartCountDown);
                        }

                        if (hit.transform.TryGetComponent(out SlotController slot))
                        {
                            if (slot.isLocked)
                            {
                                if (ACEPlay.Bridge.BridgeController.instance.IsRewardReady())
                                {
                                    UnityEvent e = new UnityEvent();
                                    e.AddListener(() =>
                                    {
                                        slot.SetLock(false);
                                    });
                                    ACEPlay.Bridge.BridgeController.instance.ShowRewarded("unlock_slot", e);
                                }
                                else
                                {
                                    UIManager.instance.ShowTextNotice("Ad is not ready!");
                                }
                                return;
                            }
                        }
                    }
                }

                if (selectedPin != null)
                {
                    Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.WorldToScreenPoint(selectedPin.transform.position).z);
                    Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
                    selectedPin.Move(worldPos);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (selectedPin != null)
                    {
                        selectedPin.MouseUp();
                        selectedPin = null;
                    }
                }
            }
        }

        public void SetUp()
        {
            gameplayStatus = GameplayStatus.None;

            board.SetActive(true);

            cam.transform.position = new Vector3(0, 2, -20f);
            cam.transform.eulerAngles = new Vector3(15, 0, 0);

            if (characterController != null)
            {
                Destroy(characterController.gameObject);
            }

            if (Manager.instance.CurrentCharacterToRescue == Character.None)
            {
                Character[] characters = (Character[])System.Enum.GetValues(typeof(Character));

                if (Manager.instance.LevelPoppyTangle == 1)
                {
                    Manager.instance.CurrentCharacterToRescue = Character.Poppy;
                }
                else
                {
                    Manager.instance.CurrentCharacterToRescue = characters[Random.Range(0, characters.Length - 1)];
                }
            }

            characterController = Instantiate(Resources.Load<CharacterController>(Path.Combine("Characters", "Prefabs", Manager.instance.CurrentCharacterToRescue.ToString())), characterParent);

            characterController.Dance();

            wallTemp.SetActive(true);

            backgroundTangle.SetActive(true);
            backgroundDailyChallenge.SetActive(false);
        }

        public void Init()
        {
            game.SetActive(true);

            wallTemp.SetActive(false);

            if (gameMode == GameMode.DailyChallenge)
            {
                backgroundTangle.SetActive(false);
                backgroundDailyChallenge.SetActive(true);

                if (characterController)
                {
                    Destroy(characterController.gameObject);
                }
            }

            solver.parameters.maxDepenetration = 1;
            solver.UpdateBackend();

            if (ropes.Count > 0)
            {
                ropes.ForEach(rope =>
                {
                    if (rope) Destroy(rope.gameObject);
                });
                ropes.Clear();
            }

            if (pins.Count > 0)
            {
                pins.ForEach(pin =>
                {
                    if (pin) Destroy(pin.gameObject);
                });
                pins.Clear();
            }

            if (slots.Count > 0)
            {
                slots.ForEach(slot =>
                {
                    if (slot) Destroy(slot.gameObject);
                });
                slots.Clear();
            }

            if (wall != null)
            {
                Destroy(wall.gameObject);
            }

            int currentLevel;
            LevelData levelData;

            if (gameMode == GameMode.Tangle)
            {
                if (Manager.instance.Chapter == 1)
                {
                    Manager.instance.LevelPoppyTangle = 1;
                }

                currentLevel = Mathf.Min(Manager.instance.LevelPoppyTangle, Manager.instance.TotalLevel);
                levelData = JsonConvert.DeserializeObject<LevelData>(Resources.Load<TextAsset>(Path.Combine("Data", $"Level_{currentLevel}")).text);
            }
            else
            {
                currentLevel = MissionManager.instance.LevelTwistedChallenge;
                levelData = JsonConvert.DeserializeObject<LevelData>(Resources.Load<TextAsset>(Path.Combine("DailyChallenge", $"Level_{currentLevel}")).text);
            }

            wall = Instantiate(Resources.Load<Wall>(Path.Combine("Walls", levelData.wallId)), game.transform);

            foreach (var slotData in levelData.slotElementDatas)
            {
                SlotController newSlot = Instantiate(Resources.Load<SlotController>(Path.Combine("Gameplay", "Slot")), slotParent);
                newSlot.transform.position = slotData.positionData.ToVector3();
                newSlot.id = slotData.slotId;
                newSlot.SetLock(slotData.isLocked);

                if (wall.isBreak)
                {
                    newSlot.SetGlass();
                }

                slots.Add(newSlot);
            }

            foreach (var pinData in levelData.pinElementDatas)
            {
                PinController newPin = Instantiate(Resources.Load<PinController>(Path.Combine("Gameplay", "Pin")), pinParent);
                newPin.id = pinData.pinId;
                newPin.SetLock(pinData.isLocked);

                if (wall.isBreak)
                {
                    newPin.model.transform.localScale = Vector3.one * 0.75f;
                }

                foreach (var slot in slots)
                {
                    if (slot.id == pinData.connectedSlotId)
                    {
                        newPin.connectedSlot = slot;
                        slot.connectedPin = newPin;
                        newPin.transform.position = slot.transform.position;
                        break;
                    }
                }

                pins.Add(newPin);
            }

            MaterialType[] materialTypes = (MaterialType[])System.Enum.GetValues(typeof(MaterialType));
            int materialIndex = 0;

            foreach (var ropeData in levelData.ropeElementsDatas)
            {
                RopeController newRope = Instantiate(Resources.Load<RopeController>(Path.Combine("Gameplay", "Chain")), ropeParent);

                foreach (var pin in pins)
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
                blueprint.resolution = ropeData.particlePositionsData.Count / (ropeData.ropeLength / 0.1f) / 2f;
                //blueprint.resolution = 0.2f;
                //blueprint.resolution = ropeData.resolution / 2f - 0.1f;
                blueprint.path.Clear();

                blueprint.path.AddControlPoint(ropeData.particlePositionsData[0].ToVector3(), Vector3.zero, Vector3.zero, Vector3.up, 0.1f, 0.1f, 1, filter, Color.white, "start");
                blueprint.path.AddControlPoint(ropeData.particlePositionsData[0].ToVector3() + Vector3.back * 0.2f, Vector3.zero, Vector3.zero, Vector3.up, 0.1f, 0.1f, 1, filter, Color.white, "start2");
                //blueprint.path.AddControlPoint(ropeData.particlePositionsData[1].ToVector3(), Vector3.zero, Vector3.zero, Vector3.up, 0.1f, 0.1f, 1, filter, Color.white, "start2");

                for (int i = 1; i < ropeData.particlePositionsData.Count - 1; i++)
                {
                    blueprint.path.AddControlPoint(ropeData.particlePositionsData[i].ToVector3(), Vector3.zero, Vector3.zero, Vector3.up, 0.1f, 0.1f, 1, filterEverything, Color.white, "mid_point");
                }

                //blueprint.path.AddControlPoint(ropeData.particlePositionsData[ropeData.particlePositionsData.Count - 2].ToVector3(), Vector3.zero, Vector3.zero, Vector3.up, 0.1f, 0.1f, 1, filter, Color.white, "end2");
                blueprint.path.AddControlPoint(ropeData.particlePositionsData[ropeData.particlePositionsData.Count - 1].ToVector3() + Vector3.back * 0.2f, Vector3.zero, Vector3.zero, Vector3.up, 0.1f, 0.1f, 1, filter, Color.white, "end2");
                blueprint.path.AddControlPoint(ropeData.particlePositionsData[ropeData.particlePositionsData.Count - 1].ToVector3(), Vector3.zero, Vector3.zero, Vector3.up, 0.1f, 0.1f, 1, filter, Color.white, "end");

                blueprint.path.FlushEvents();
                blueprint.GenerateImmediate();

                newRope.obiRope.ropeBlueprint = blueprint;

                var attachmentStart = newRope.gameObject.AddComponent<ObiParticleAttachment>();
                //var attachmentStart2 = newRope.gameObject.AddComponent<ObiParticleAttachment>();
                var attachmentEnd = newRope.gameObject.AddComponent<ObiParticleAttachment>();
                //var attachmentEnd2 = newRope.gameObject.AddComponent<ObiParticleAttachment>();

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

                ropes.Add(newRope);
            }

            UIManager.instance.ShowUIIngame();

            EventManager.EmitEventData(EventVariables.CountDown, levelData.duration);
            EventManager.EmitEvent(EventVariables.CountDownShowAdsInGame);

            if (cam.transform.position != new Vector3(0f, -1.37f, -10f))
            {
                cam.transform.DOMove(new Vector3(0f, -1.37f, -10f), 1f);
                cam.transform.DORotate(Vector3.zero, 1f, RotateMode.Fast).OnComplete(() =>
                {
                    solver.parameters.maxDepenetration = 10f;

                    CheckTutorial();

                    gameplayStatus = GameplayStatus.Playing;
                });
            }
            else
            {
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    solver.parameters.maxDepenetration = 10f;

                    CheckTutorial();

                    gameplayStatus = GameplayStatus.Playing;
                });
            }

            ACEPlay.Bridge.BridgeController.instance.LogLevelStartWithParameter("tangle", currentLevel);
        }

        public SlotController GetNearestSlot(PinController pin)
        {
            SlotController nearestSlot = null;

            float distance = float.MaxValue;
            foreach (var slot in slots)
            {
                if (!slot.isLocked)
                {
                    float dist = Vector2.Distance(pin.transform.position, slot.transform.position);
                    if (dist < distance)
                    {
                        distance = dist;
                        nearestSlot = slot;
                    }
                }
            }

            return nearestSlot;
        }

        public void SpawnVFXMerge(Vector3 pos)
        {
            Instantiate(vfxMerge, pos, Quaternion.identity, effectParent);
        }

        public bool CanMerge()
        {
            foreach (var pin in pins)
            {
                if (pin != null && pin.isMoving)
                {
                    return false;
                }
            }

            return true;
        }

        Coroutine countDownCanMerge = null;
        IEnumerator IE_CountDownCanMerge()
        {
            yield return new WaitForSeconds(0.5f);
            canMerge = true;

            countDownCanMerge = null;
        }

        public void SetCanMerge(bool _canMerge)
        {
            if (_canMerge)
            {
                countDownCanMerge = StartCoroutine(IE_CountDownCanMerge());
            }
            else
            {
                canMerge = false;

                if (countDownCanMerge != null)
                {
                    StopCoroutine(countDownCanMerge);
                    countDownCanMerge = null;
                }
            }
        }

        public void CheckWin()
        {
            if (IsWin())
            {
                if (gameplayStatus != GameplayStatus.Win && gameplayStatus != GameplayStatus.Lose)
                {
                    gameplayStatus = GameplayStatus.Win;

                    if (gameMode == GameMode.Tangle)
                    {
                        Manager.instance.LevelPoppyTangle++;
                        Manager.instance.Stage++;

                        List<Character> rescuedChars = Manager.instance.RescuedCharacter;
                        rescuedChars.Add(characterController.character);
                        Manager.instance.RescuedCharacter = rescuedChars;

                        Manager.instance.CurrentCharacterToRescue = Character.None;

                        EventManager.SetDataGroup(EventVariables.UpdateMission, MissionType.RescuePoppy, 1);
                        EventManager.EmitEvent(EventVariables.UpdateMission);
                        EventManager.EmitEvent(EventVariables.StopCountDown);
                        EventManager.EmitEventData(EventVariables.EndGame, true);

                        foreach (var slot in slots)
                        {
                            slot.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
                            {
                                Destroy(slot.gameObject);
                            });
                        }

                        DOVirtual.DelayedCall(0.5f, () =>
                        {
                            slots.Clear();

                            wall.Action(() =>
                            {
                                cam.transform.DORotate(new Vector3(15, 0, 0), 1f);
                                cam.transform.DOMove(new Vector3(0, 2, -20f), 1f).OnComplete(() =>
                                {
                                    characterController.Run();
                                });
                            });
                        });

                        ACEPlay.Bridge.BridgeController.instance.LogLevelCompleteWithParameter("tangle", Manager.instance.LevelPoppyTangle - 1);
                    }
                    else
                    {
                        int levelChallenge = MissionManager.instance.LevelTwistedChallenge;
                        MissionManager.instance.LevelTwistedChallenge = levelChallenge + 1;
                        MissionManager.instance.TwistedChallengePlayCount++;

                        UIManager.instance.ShowUIWinChallenge();

                        ACEPlay.Bridge.BridgeController.instance.LogLevelCompleteWithParameter("challenge", levelChallenge);
                    }
                }
            }
        }

        public bool IsWin()
        {
            foreach (var rope in ropes)
            {
                if (rope != null && !rope.isMerge)
                {
                    return false;
                }
            }

            return true;
        }

        public void QuitGame()
        {
            gameplayStatus = GameplayStatus.None;

            if (ropes.Count > 0)
            {
                ropes.ForEach(rope =>
                {
                    if (rope) Destroy(rope.gameObject);
                });
                ropes.Clear();
            }

            if (pins.Count > 0)
            {
                pins.ForEach(pin =>
                {
                    if (pin) Destroy(pin.gameObject);
                });
                pins.Clear();
            }

            if (slots.Count > 0)
            {
                slots.ForEach(slot =>
                {
                    if (slot) Destroy(slot.gameObject);
                });
                slots.Clear();
            }

            if (wall != null)
            {
                Destroy(wall.gameObject);
            }

            if (characterController != null)
            {
                Destroy(characterController.gameObject);
            }

            UIManager.instance.UIInGame.hand.DOKill();

            solver.UpdateBackend();

            game.SetActive(false);
            board.SetActive(false);
        }

        public void CutRope()
        {
            Vector3 mousePos = (Vector3)EventManager.GetData(EventVariables.CutRope);

            Ray ray = cam.ScreenPointToRay(mousePos);

            int filter = ObiUtils.MakeFilter(ObiUtils.CollideWithEverything, 0);

            // perform a raycast, check if it hit anything:
            if (solver.Raycast(ray, out QueryResult result, filter, 200, 0.1f))
            {
                // get the start and size of the simplex that was hit:
                int simplexStart = solver.simplexCounts.GetSimplexStartAndSize(result.simplexIndex, out int simplexSize);

                int particleIndex = solver.simplices[simplexStart];
                ObiRope rope = solver.particleToActor[particleIndex].actor as ObiRope;

                Vector3 pos = solver.transform.TransformPoint(solver.positions[particleIndex]);

                ObiStructuralElement elementToCut = null;
                foreach (var element in rope.elements)
                {
                    if (element.particle1 == particleIndex || element.particle2 == particleIndex)
                    {
                        elementToCut = element;
                        break;
                    }
                }

                if (elementToCut != null)
                {
                    int axe = Manager.instance.Axe;
                    Manager.instance.Axe = axe - 1;

                    EventManager.SetDataGroup(EventVariables.UpdateMission, MissionType.UseBooster, 1);
                    EventManager.EmitEvent(EventVariables.UpdateMission);

                    if (UIManager.instance.UIUseBooster != null)
                    {
                        UIManager.instance.UIUseBooster.Hide();
                    }

                    GameObject newAxe = axePrefab.Spawn(board.transform);
                    newAxe.transform.position = pos;

                    DOVirtual.DelayedCall(0.25f, () =>
                    {
                        rope.Tear(elementToCut);
                        rope.RebuildConstraintsFromElements();

                        rope.stretchingScale = 0;

                        DOVirtual.DelayedCall(0.4f, () =>
                        {
                            rope.GetComponent<RopeController>().Merge();

                            newAxe.Recycle();
                        }, false);

                        AudioController.instance.PlaySoundBoosterAxe();
                    }, false);
                }
            }
        }

        public void BreakLockedPin()
        {
            Vector3 mousePos = (Vector3)EventManager.GetData(EventVariables.BreakLockedPin);
            Ray ray = cam.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.TryGetComponent(out PinController pinToUnlock) && pinToUnlock.isLocked)
                {
                    int hammer = Manager.instance.Hammer;
                    Manager.instance.Hammer = hammer - 1;

                    EventManager.SetDataGroup(EventVariables.UpdateMission, MissionType.UseBooster, 1);
                    EventManager.EmitEvent(EventVariables.UpdateMission);

                    EventManager.SetData(EventVariables.BreakLockedPin, pinToUnlock);

                    HammerController newHammer = hammerPrefab.Spawn(board.transform).GetComponent<HammerController>();
                    newHammer.Play();

                    if (UIManager.instance.UIUseBooster != null)
                    {
                        UIManager.instance.UIUseBooster.Hide();
                    }
                }
            }
        }

        public void CheckTutorial()
        {
            Transform hand = UIManager.instance.UIInGame.hand;

            if (Manager.instance.LevelPoppyTangle == 1 && gameMode == GameMode.Tangle)
            {
                hand.gameObject.SetActive(true);

                Vector3 pinPos = CanvasPositioningExtensions.WorldToCanvasPosition(UIManager.instance.canvasUI1, pins[1].transform.position, cam);
                hand.localPosition = new Vector3(pinPos.x, pinPos.y, 0);
                hand.localScale = Vector3.one;

                hand.DOScale(0.8f, 0.5f).SetEase(Ease.InQuad).OnComplete(() =>
                {
                    Vector3 slotPos = CanvasPositioningExtensions.WorldToCanvasPosition(UIManager.instance.canvasUI1, slots[slots.Count - 1].transform.position, cam);

                    hand.DOLocalMove(new Vector3(slotPos.x, slotPos.y, 0), 1f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        hand.DOScale(1f, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
                        {
                            CheckTutorial();
                        });
                    });
                });
            }
            else
            {
                hand.gameObject.SetActive(false);
                hand.DOKill();
            }
        }
    }
}