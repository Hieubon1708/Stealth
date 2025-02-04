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
        bool isXIncrease;
        bool isYIncrease;
        public float radius;
        float distance;

        private void Start()
        {
            isXIncrease = Random.Range(0, 2) == 0;
            isYIncrease = Random.Range(0, 2) == 0;
            RandomExtra();
        }

        public void Init(float radius, Vector3 pos)
        {
            this.radius = radius;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(pos, out hit, 100, NavMesh.AllAreas))
            {
                Agent.Warp(hit.position);
            }
            else BridgeController.instance.Debug_LogWarning("!");
        }

        void RandomExtra()
        {
            isXIncrease = !isXIncrease;
            isYIncrease = !isYIncrease;

            Invoke("RandomExtra", Random.Range(0.5f, 2f));
        }

        public void MoveTo(Vector3 Position)
        {
            if (!Agent.enabled) return;
            distance = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
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
            float mouseMagnitude = PlayerController.instance.playerTouchMovement.scaledMovement.magnitude / 10;
            extraX = Mathf.Clamp(isXIncrease ? extraX + mouseMagnitude : extraX - mouseMagnitude, -0.25f, 0.25f);
            extraY = Mathf.Clamp(isYIncrease ? extraY + mouseMagnitude : extraY - mouseMagnitude, -0.25f, 0.25f);
        }
    }
}