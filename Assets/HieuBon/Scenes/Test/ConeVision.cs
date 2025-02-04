using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public Material VisionConeMaterial;
    public float VisionRange;
    public float VisionAngle;
    public LayerMask VisionObstructingLayer;
    public int VisionConeResolution = 120;
    Mesh VisionConeMesh;
    MeshFilter MeshFilter_;

    void Start()
    {
        gameObject.AddComponent<MeshRenderer>().material = VisionConeMaterial;
        MeshFilter_ = gameObject.AddComponent<MeshFilter>();
        VisionConeMesh = new Mesh();
        VisionAngle *= Mathf.Deg2Rad;
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
        Vertices[0] = Vector3.zero;
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
            if (Physics.Raycast(transform.position, RaycastDirection, out hit, VisionRange, VisionObstructingLayer))
            {
                Vertices[i + 1] = VertForward * hit.distance;
            }
            else
            {
                Vertices[i + 1] = VertForward * VisionRange;
            }
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
        MeshFilter_.mesh = VisionConeMesh;
    }


    void CheckEnemyInVisionCone()
    {
        for (int i = 1; i < VisionConeResolution; i++)
        {
            Vector3 direction = VisionConeMesh.vertices[i].normalized;
            float distance = Vector3.Distance(transform.position, VisionConeMesh.vertices[i]);
            RaycastHit hit;
           if(i == 1)  Debug.DrawLine(transform.position, direction * VisionRange, Color.yellow);
            if (Physics.Raycast(transform.position, direction, out hit, VisionRange))
            {
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    Debug.Log("Enemy detected in vision cone!");
                }
            }
        }
    }
}