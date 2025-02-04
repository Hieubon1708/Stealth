using UnityEngine;

namespace Hunter
{
    public class LaserLine : MonoBehaviour
    {
        public ParticleSystem laserDamage;

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                laserDamage.Play();
                Player player = GameController.instance.GetPoppy(other.gameObject);
                player.Die(other.transform);
                GameController.instance.Alert(GameController.AlertType.Laser, gameObject);
            }
        }
    }
}
