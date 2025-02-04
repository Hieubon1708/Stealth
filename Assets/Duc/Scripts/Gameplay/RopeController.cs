using DG.Tweening;
using Obi;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Duc.PoppyTangle
{
    public class RopeController : MonoBehaviour
    {
        public ObiRope obiRope;
        public ObiRopeCursor obiRopeCursor;
        public ObiRopeChainRenderer obiRopeChainRenderer;

        public PinController startPin;
        public PinController endPin;

        public float ropeLength;
        public float ropeRestLength;
        public float ropeStrain;

        public MeshRenderer meshRenderer;
        public MaterialType materialType;

        public bool isMerge = false;

        private Color defaultColor;
        private Color tensionColor;

        private void Awake()
        {
            if (obiRope == null) obiRope = GetComponent<ObiRope>();
            if (obiRopeCursor == null) obiRopeCursor = GetComponent<ObiRopeCursor>();
        }

        // Start is called before the first frame update
        void Start()
        {
            ropeRestLength = obiRope.restLength;
        }

        // Update is called once per frame
        void Update()
        {
            ropeLength = obiRope.CalculateLength();
            ropeStrain = ropeRestLength / ropeLength;

            //if (ropeStrain < 0.7f)
            //{
            //    if (meshRenderer != null)
            //    {
            //        meshRenderer.material.SetColor("_BaseColor", tensionColor);
            //    }
            //    else
            //    {
            //        foreach (var link in obiRopeChainRenderer.linkInstances)
            //        {
            //            link.GetComponentInChildren<Renderer>().material.SetColor("_BaseColor", tensionColor);
            //        }
            //    }
            //}
            //else
            //{
            //    if (meshRenderer != null)
            //    {
            //        meshRenderer.material.SetColor("_BaseColor", defaultColor);
            //    }
            //    else
            //    {
            //        foreach (var link in obiRopeChainRenderer.linkInstances)
            //        {
            //            link.GetComponentInChildren<Renderer>().material.SetColor("_BaseColor", defaultColor);
            //        }
            //    }
            //}
        }

        public void SetColor(MaterialType _materialType)
        {
            materialType = _materialType;

            GameObject linkPrefab = Resources.Load<GameObject>(Path.Combine("Chains", materialType.ToString()));

            obiRopeChainRenderer.linkPrefabs.Clear();
            obiRopeChainRenderer.linkPrefabs.Add(linkPrefab);

            //Material colorMat = linkPrefab.GetComponentInChildren<MeshRenderer>().material;

            //if (meshRenderer == null)
            //{
            //    colorMat = Resources.Load<Material>(Path.Combine("Materials", "Chain", _materialType.ToString()));
            //    StartCoroutine(IE_SetColor(colorMat));
            //}
            //else
            //{
            //    colorMat = Resources.Load<Material>(Path.Combine("Materials", "Rope", _materialType.ToString()));
            //    meshRenderer.material = colorMat;
            //}

            //defaultColor = colorMat.GetColor("_BaseColor");
            //tensionColor = (Color.red + defaultColor) / 2f;
        }

        IEnumerator IE_SetColor(Material material)
        {
            yield return new WaitForEndOfFrame();

            foreach (var link in obiRopeChainRenderer.linkInstances)
            {
                link.GetComponentInChildren<Renderer>().material = material;
            }
        }

        public void ChangeLength(bool increase)
        {
            float speed = 2f;

            if (increase)
            {
                obiRopeCursor.ChangeLength(obiRope.restLength + speed * Time.deltaTime);
            }
            else
            {
                obiRopeCursor.ChangeLength(obiRope.restLength - speed * Time.deltaTime);
            }
        }

        public bool IsRopeMaxLength(bool mouseUp)
        {
            if (LevelGenerator.instance != null) return false;

            if (!mouseUp)
            {
                return ropeStrain <= 0.65f;
            }
            else
            {
                return ropeStrain <= 0.7f;
            }
        }

        public void Merge()
        {
            if (GameplayController.instance.gameplayStatus != GameplayStatus.Playing) return;

            if (GameplayController.instance.CanMerge() && GameplayController.instance.canMerge && !isMerge)
            {
                isMerge = true;

                //RopeCollisionDetector.instance.ropes.Remove(obiRope);

                startPin.connectedSlot.connectedPin = null;
                endPin.connectedSlot.connectedPin = null;

                float speedChangeLength = obiRope.restLength / 0.25f;

                startPin.transform.DORotate(Vector3.up * 180f, 0.25f, RotateMode.LocalAxisAdd);
                startPin.transform.DOMove(endPin.transform.position + Vector3.back * 0.75f, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    //Destroy(obiRope.gameObject);
                    GameplayController.instance.SpawnVFXMerge(endPin.transform.position + Vector3.back * 0.5f);

                    startPin.transform.DOScale(0f, 0.25f).SetEase(Ease.InBack);
                    endPin.transform.DOScale(0f, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
                    {
                        GameplayController.instance.CheckWin();

                        Destroy(startPin.gameObject);
                        Destroy(endPin.gameObject);
                        Destroy(gameObject);
                    });

                    AudioController.instance.PlaySoundMerge();
                }).OnUpdate(() =>
                {
                    //if (obiRope.CalculateLength() > 2f)
                    //{
                    obiRopeCursor.ChangeLength(obiRope.restLength - speedChangeLength * Time.deltaTime);
                    obiRope.RebuildConstraintsFromElements();
                    //}
                });

                if (Manager.instance.LevelPoppyTangle == 1)
                {
                    UIManager.instance.UIInGame.hand.DOKill();
                    UIManager.instance.UIInGame.hand.gameObject.SetActive(false);
                }
            }
        }

        private void OnDestroy()
        {
            GameplayController.instance.ropes.Remove(this);
            //RopeCollisionDetector.instance.solver.UpdateBackend();
        }
    }
}