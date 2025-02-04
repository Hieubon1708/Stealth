using ACEPlay.Bridge;
using Duc;
using System.Collections;
using TigerForge;
using UnityEngine;
using UnityEngine.AI;

namespace Hunter
{
    public abstract class Bot : MonoBehaviour
    {
        public int startHp;
        public int hp;
        protected int indexPath;

        public RadarView radarView;
        public Animator animator;
        public NavMeshAgent navMeshAgent;
        public PathInfo pathInfo;
        public ParticleSystem blood;
        public CapsuleCollider col;
        public Transform hips;
        public Rigidbody[] rbs;
        public GameObject scream;

        public float time;
        public float speed;
        public float rotateSpeed;
        public float detectSpeed;
        public float rotateDetectSpeed;

        public bool isKilling;
        public bool isDodging;

        protected Coroutine probe;
        protected Coroutine attack;

        public BotWeapon weapon;

        public void Init(PathInfo pathInfo)
        {
            this.pathInfo = pathInfo;
        }

        public float GetSpeed()
        {
            return col.enabled ? navMeshAgent.speed : rbs[0].velocity.magnitude;
        }

        public void StopProbe()
        {
            if (probe != null)
            {
                StopCoroutine(probe);
                probe = null;
            }
        }

        public void StartProbe(int currentIndex)
        {
            if (probe == null) probe = StartCoroutine(Probe(currentIndex));
        }

        public void StopAttack()
        {
            if (attack != null)
            {
                isKilling = false;
                animator.SetTrigger("NoAiming");
                StopCoroutine(attack);
                attack = null;
            }
        }

        public void StartAttack(GameObject target)
        {
            if (isKilling || attack != null) return;
            animator.ResetTrigger("NoAiming");
            animator.ResetTrigger("Fire");
            attack = StartCoroutine(Attack(target));
            isKilling = true;
        }

        public void ChangeSpeed(float speed, float rotateSpeed)
        {
            navMeshAgent.speed = speed;
            navMeshAgent.speed = rotateSpeed;
        }

        public virtual void SubtractHp(int hp, Transform killer)
        {
            if (this.hp <= 0)
            {
                EventManager.SetDataGroup(EventVariables.UpdateMission, this as BossBot ? MissionType.KillBoss : MissionType.KillEnemy, 1);
                EventManager.EmitEvent(EventVariables.UpdateMission);
                int coin = 0;
                if(this as NormalBot || this as DemolitionBot)
                {
                    coin = Random.Range(1, 3);
                    BridgeController.instance.Debug_Log("Enemy_1 " + coin);
                }
                else if(this as SniperBot)
                {
                    coin = Random.Range(1, 4);
                    BridgeController.instance.Debug_Log("Enemy_2 " + coin);
                }
                else if(this as BossBot)
                {
                    coin = Random.Range(4, 9);
                    BridgeController.instance.Debug_Log("Boss " + coin);
                }
                else if(this as MiniBoss)
                {
                    coin = Random.Range(3, 6);
                    BridgeController.instance.Debug_Log("MiniBoss " + coin);
                }
                else
                {
                    coin = 1;
                    BridgeController.instance.Debug_Log("!!! " + this);
                }
                Player player = GameController.instance.GetPoppy(killer.gameObject);
                if (player == null) player = GameController.instance.poppies[Random.Range(0, GameController.instance.poppies.Count)];
                if (player != null) PlayerController.instance.PlayDollars(player.fxPool, new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), coin);
            }
        }

        public int index;

        IEnumerator Probe(int currentIndex)
        {
            if (pathInfo.paths[0].Length > 1)
            {
                ChangeSpeed(speed, rotateSpeed);
                index = currentIndex;
                if (pathInfo.isUpdatePosition)
                {
                    if (pathInfo.pathType == GameController.PathType.Circle)
                    {
                        while (col.enabled)
                        {
                            Vector3 direction = new Vector3(pathInfo.paths[indexPath][index].x, transform.position.y, pathInfo.paths[indexPath][index].z) - transform.position;
                            Quaternion targetRotation = Quaternion.LookRotation(direction);
                            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
                            {
                                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed);
                                yield return new WaitForFixedUpdate();
                            }
                            yield return new WaitForSeconds(time);
                            animator.SetBool("Walking", true);
                            navMeshAgent.destination = pathInfo.paths[indexPath][index];
                            yield return new WaitForFixedUpdate();
                            yield return new WaitForFixedUpdate();
                            yield return new WaitForFixedUpdate();
                            while (col.enabled)
                            {
                                if (navMeshAgent.remainingDistance <= 0.1f) animator.SetBool("Walking", false);
                                if (navMeshAgent.remainingDistance == navMeshAgent.stoppingDistance) break;
                                yield return new WaitForFixedUpdate();
                            }
                            yield return new WaitForSeconds(time);
                            if (index == pathInfo.paths[indexPath].Length - 1) index = 0;
                            else index++;
                        }
                    }
                    else if (pathInfo.pathType == GameController.PathType.Repeat)
                    {
                        bool isIncrease = true;
                        while (col.enabled)
                        {
                            Vector3 direction = new Vector3(pathInfo.paths[indexPath][index].x, transform.position.y, pathInfo.paths[indexPath][index].z) - transform.position;
                            Quaternion targetRotation = Quaternion.LookRotation(direction);
                            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
                            {
                                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed);
                                yield return new WaitForFixedUpdate();
                            }
                            yield return new WaitForSeconds(time);
                            animator.SetBool("Walking", true);
                            navMeshAgent.destination = pathInfo.paths[indexPath][index];
                            yield return new WaitForFixedUpdate();
                            yield return new WaitForFixedUpdate();
                            yield return new WaitForFixedUpdate();
                            while (col.enabled)
                            {
                                if (navMeshAgent.remainingDistance <= 0.1f) animator.SetBool("Walking", false);
                                if (navMeshAgent.remainingDistance == navMeshAgent.stoppingDistance) break;
                                yield return new WaitForFixedUpdate();
                            }
                            yield return new WaitForSeconds(time);
                            if (index == pathInfo.paths[indexPath].Length - 1 || index == 0) isIncrease = !isIncrease;
                            if (isIncrease) index++;
                            else index--;
                        }
                    }
                }
                else
                {
                    if (pathInfo.pathType == GameController.PathType.Circle)
                    {
                        while (col.enabled)
                        {
                            Vector3 direction = new Vector3(pathInfo.paths[indexPath][index].x, transform.position.y, pathInfo.paths[indexPath][index].z) - transform.position;
                            Quaternion targetRotation = Quaternion.LookRotation(direction);
                            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
                            {
                                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed);
                                yield return new WaitForFixedUpdate();
                            }
                            yield return new WaitForSeconds(time);
                            if (index == pathInfo.paths[indexPath].Length - 1) index = 1;
                            else index++;
                        }
                    }
                    else if (pathInfo.pathType == GameController.PathType.Repeat)
                    {
                        bool isIncrease = false;
                        while (col.enabled)
                        {
                            float startRotate = transform.right.x;
                            Vector3 direction = new Vector3(pathInfo.paths[indexPath][index].x, transform.position.y, pathInfo.paths[indexPath][index].z) - transform.position;
                            Quaternion targetRotation = Quaternion.LookRotation(direction);
                            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
                            {
                                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed);
                                yield return new WaitForFixedUpdate();
                            }
                            yield return new WaitForSeconds(time);
                            if (index == pathInfo.paths[indexPath].Length - 1 || index == 1) isIncrease = !isIncrease;
                            if (isIncrease) index++;
                            else index--;
                        }
                    }
                }
            }
            else
            {
                Quaternion targetRotation = Quaternion.Euler(0, pathInfo.angle, 0);
                while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed);
                    yield return new WaitForFixedUpdate();
                }
                yield return new WaitForSeconds(time);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("BlastZone"))
            {
                SubtractHp(100, other.transform);
            }
        }


        public abstract IEnumerator Attack(GameObject target);

        public IEnumerator Die()
        {
            name = "Die";
            scream.SetActive(true);
            yield return new WaitForFixedUpdate();
            scream.SetActive(false);
        }

        public void IsKinematic(bool isKinematic)
        {
            if (rbs.Length == 0) rbs = hips.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < rbs.Length; i++)
            {
                rbs[i].isKinematic = isKinematic;
            }
        }

        public void PlayBlood()
        {
            blood.Play();
        }

        public abstract void ResetBot();
    }
}