using ACEPlay.Bridge;
using UnityEngine;
using UnityEngine.AI;

namespace Hunter
{
    [RequireComponent(typeof(NavMeshAgent))]
    [DefaultExecutionOrder(1)]
    public class AIUnit : MonoBehaviour
    {
        public NavMeshAgent Agent;
        public Animator animator;
        public Player player;
        public float extraX;
        public float extraY;
        public float radius;
        float distance;

        public void Init(float radius, Vector3 pos)
        {
            this.radius = radius;
            transform.position = pos;
            Agent.enabled = true;
        }

        public void MoveTo(Vector3 Position)
        {
            if (!Agent.enabled) return;
            distance = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
            Agent.speed = UIController.instance.layerCover.raycastTarget ? 7 : PlayerController.instance.playerTouchMovement.GetSpeedAgent();
            if (Agent.path.corners.Length < 6 && distance < radius * 1.5f)
            {
                Agent.stoppingDistance = 0;
                Agent.destination = Position;
            }
            else
            {
                Agent.stoppingDistance = radius;
                Agent.destination = PlayerController.instance.transform.position;
            }
        }

        public void Update()
        {
            if (!player.col.enabled) return;
            Vector3 lookAt = player.isKilling ? player.lookAt.transform.position : transform.position + PlayerController.instance.playerTouchMovement.scaledMovement;
            transform.LookAt(lookAt);
            float speed = PlayerController.instance.GetSpeed(player);
            animator.SetFloat("Speed", Mathf.Clamp01(speed));
        }
    }
}