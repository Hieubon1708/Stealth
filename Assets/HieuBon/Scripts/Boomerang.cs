using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hunter
{
    public class Boomerang : Bullet
    {
        public override void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Player player = GameController.instance.GetPoppy(other.gameObject);
                if (player != null)
                {
                    player.Die(transform);
                }
            }
            if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                gameObject.SetActive(false);
            }
        }

        public void Update()
        {
            transform.Rotate(Vector3.right * 5);
        }
    }
}
