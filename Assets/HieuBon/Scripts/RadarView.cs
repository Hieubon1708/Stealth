using System.Collections.Generic;
using UnityEngine;

namespace Hunter
{
    public sealed class RadarView : MonoBehaviour
    {
        public float VisionRange;
        public float VisionAngle;
        public LayerMask VisionObstructingLayer;
        public int VisionConeResolution = 120;
        Mesh VisionConeMesh;
        public MeshFilter MeshFilter;
        public MeshRenderer MeshRenderer;
        public GameObject target;
        public Material white;
        public Material red;
        public List<GameObject> allies = new List<GameObject>();
        LayerMask layer;
        Vector3[] dirs;
        public SentryBot sentryBot;

        void Awake()
        {
            VisionConeMesh = new Mesh();
            VisionAngle *= Mathf.Deg2Rad;
            layer = LayerMask.GetMask("Player", "Wall", "Character");
            dirs = new Vector3[VisionConeResolution];
        }

        private void Start()
        {
            SetColor(false);
        }

        public void SetColor(bool isRed)
        {
            MeshRenderer.material = isRed ? red : white;
        }

        void Update()
        {
            DrawVisionCone();
            CheckEnemyInVisionCone();
        }

        void DrawVisionCone()
        {
            int[] triangles = new int[(VisionConeResolution - 1) * 3];
            Vector3[] Vertices = new Vector3[VisionConeResolution + 1];
            Vector2[] UVs = new Vector2[Vertices.Length];
            Vertices[0] = Vector3.zero;
            UVs[0] = new Vector2(0.5f, 0.5f);
            float Currentangle = -VisionAngle / 2;
            float angleIcrement = VisionAngle / (VisionConeResolution - 1);
            float Sine;
            float Cosine;

            for (int i = 0; i < VisionConeResolution; i++)
            {
                Sine = Mathf.Sin(Currentangle);
                Cosine = Mathf.Cos(Currentangle);
                Vector3 RaycastDirection = (transform.forward * Cosine) + (transform.right * Sine);
                Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);
                RaycastHit hit;
                dirs[i] = RaycastDirection;
                if (Physics.Raycast(transform.position, RaycastDirection, out hit, VisionRange, VisionObstructingLayer))
                {
                    Vertices[i + 1] = VertForward * hit.distance;
                }
                else
                {
                    Vertices[i + 1] = VertForward * VisionRange;
                }

                UVs[i + 1] = new Vector2((Vertices[i + 1].x / VisionRange + 1) / 2, (Vertices[i + 1].z / VisionRange + 1) / 2);
                Currentangle += angleIcrement;
            }

            for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
            {
                triangles[i] = 0;
                triangles[i + 1] = j + 1;
                triangles[i + 2] = j + 2;
            }

            VisionConeMesh.Clear();
            VisionConeMesh.vertices = Vertices;
            VisionConeMesh.triangles = triangles;
            VisionConeMesh.uv = UVs;
            MeshFilter.mesh = VisionConeMesh;
        }

        void CheckEnemyInVisionCone()
        {
            GameObject target = null;
            bool isContainTarget = false;
            for (int i = 1; i < VisionConeResolution; i++)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, dirs[i], out hit, VisionRange, layer))
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.CompareTag("Player"))
                        {
                            target = hit.collider.gameObject;
                            if (this.target == target)
                            {
                                isContainTarget = true;
                            }
                        }
                        if (sentryBot != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Character") && !hit.collider.attachedRigidbody.isKinematic && !allies.Contains(hit.collider.gameObject))
                        {
                            Bot bot = GameController.instance.GetBoneNBot(allies, hit.collider.gameObject);
                            if (bot != null) sentryBot.StartHear(bot.rbs[0].gameObject);
                        }
                    }
                }
            }
            if (target != null)
            {
                if (!isContainTarget)
                {
                    this.target = target;
                }
            }
            else
            {
                //BridgeController.instance.Debug_Log("break");
                this.target = null;
            }
        }
    }
}