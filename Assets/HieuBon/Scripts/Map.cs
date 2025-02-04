using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Map : MonoBehaviour
{
    public NavMeshData navMeshData;

    public void Awake()
    {
        NavMeshSurface navMeshSurface = GetComponent<NavMeshSurface>();
        navMeshSurface.navMeshData = navMeshData;
        navMeshSurface.AddData();
    }
}
