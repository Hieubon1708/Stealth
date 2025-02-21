using UnityEngine;

namespace Hunter
{
    public class BotHealth : MonoBehaviour
    {
        public SpriteRenderer[] bloodIcons;

        public void SubtractHp(int currentHp)
        {
            for (int i = 0; i < bloodIcons.Length; i++)
            {
                if(currentHp == 0)
                {
                    bloodIcons[i].gameObject.SetActive(false);
                    continue;
                }
                if (i < currentHp)
                {
                    bloodIcons[i].color = Color.white;
                }
                else
                {
                    bloodIcons[i].color = Color.black;
                }
            }
        }

        void Update()
        {
            if (GameController.instance != null)
            {
                transform.LookAt(new Vector3(transform.position.x, GameController.instance.cam.transform.position.y, GameController.instance.cam.transform.position.z));
            }
        }
    }
}
