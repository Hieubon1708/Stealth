using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform pre;

    public void Start()
    {
        Transform[] transforms = pre.GetChild(0).GetComponentsInChildren<Transform>();

        for (int i = 0; i < transforms.Length; i++)
        {
            transforms[i].name = "mixamorig:" + transforms[i].name;
        }
    }
}
