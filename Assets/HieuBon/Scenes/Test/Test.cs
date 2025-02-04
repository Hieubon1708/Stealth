using UnityEngine;

public class Test : MonoBehaviour
{
    public Animator ani;

    public void Start()
    {
        ani.SetBool("Walking", true);
    }
}
