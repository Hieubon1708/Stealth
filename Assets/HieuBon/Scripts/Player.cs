using ACEPlay.Bridge;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

namespace Hunter
{
    public class Player : MonoBehaviour
    {
        public GameManager.Character character;
        public GameController.WeaponType WeaponType;
        public Animator animator;
        public NavMeshAgent navMeshAgent;
        public GameObject lookAt;
        public ParticleSystem blood;
        public CapsuleCollider attackRangeCollider;
        public Transform hand;
        public bool isKilling;
        public Transform hips;
        public Rigidbody[] rbs;
        public CapsuleCollider col;
        public Weapon weapon;
        public AIUnit aIUnit;
        public float xExtraRadius;
        public float yExtraRadius;
        public SkinnedMeshRenderer meshRenderer;
        public List<GameObject> bots = new List<GameObject>();
        Tween delayKill;
        Material defaultMaterial;
        public Outline outline;
        int amountSmoke;
        LayerMask layer;
        public GameObject fxPool;
        public bool isAttack;

        private void Start()
        {
            defaultMaterial = meshRenderer.material;
            layer = LayerMask.GetMask("Bot", "Wall");
        }

        public void Init(Weapon weapon)
        {
            this.weapon = weapon;
            attackRangeCollider.radius = weapon.attackRange;
            DOVirtual.DelayedCall(0.06f, delegate
            {
                rbs = hips.GetComponentsInChildren<Rigidbody>();
                IsKinematic(true);
            });
        }

        public void LoadWeapon(GameController.WeaponType weaponType)
        {
            GameController.instance.weaponEquip.Equip(this, weaponType);
        }

        public void OnTriggerStay(Collider other)
        {
            if (!col.enabled || weapon == null) return;
            if (other.CompareTag("Bot"))
            {
                RaycastHit hit;
                Vector3 from = transform.position;
                Vector3 to = other.transform.position;
                from.y += 0.5f;
                to.y = from.y;
                Physics.Linecast(from, to, out hit, layer);
                if (hit.collider != null && hit.collider.CompareTag("Bot"))
                {
                    if (!bots.Contains(other.gameObject))
                    {
                        bots.Add(other.gameObject);
                    }
                    if (isKilling) return;
                    isKilling = true;
                    isAttack = true;
                    lookAt = other.gameObject;
                    animator.SetInteger("Hit Style", Random.Range(0, 2));
                    animator.SetTrigger("Hit");
                    PlayerController.instance.playerTouchMovement.navMeshAgent.speed = 5.5f;
                    delayKill = DOVirtual.DelayedCall(0.35f, delegate
                    {
                        isAttack = false;
                        AudioController.instance.PlaySoundNVibrate(AudioController.instance.cut, 0);
                        ChangeLookAt();
                        if (!GameController.instance.IsAttack()) PlayerController.instance.playerTouchMovement.navMeshAgent.speed = 7;
                        for (int i = 0; i < bots.Count; i++)
                        {
                            Bot bot = GameController.instance.GetBot(bots[i].gameObject);
                            if (bot != null) bot.SubtractHp(weapon.damage, transform);
                        }
                        DOVirtual.DelayedCall(0.35f, delegate
                        {
                            isKilling = false;
                            bots.Clear();
                        });
                    });
                }
            }
        }

        public void TakeMoney(int money)
        {
            PlayerController.instance.takeMoney.TakeOn(money);
            PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money") + money);
        }

        public void Die(Transform killer)
        {
            //if (GameController.instance.bots.Count == 0 && UIController.instance.gamePlay.tempStageType == StageType.StealthBoss || !col.enabled) return;
            UIController.instance.virtualCam.StartShakeCam(2.5f);
            AudioController.instance.PlaySoundNVibrate(AudioController.instance.playerDie, 75);
            PlayBlood();
            PlayerController.instance.IsHasKey(gameObject);
            GameController.instance.RemovePoppy(this);
            delayKill.Kill();
            isKilling = false;
            if (!GameController.instance.IsAttack()) PlayerController.instance.playerTouchMovement.navMeshAgent.speed = 7;
            CancelInvoke(nameof(ChangeLookAt));
            UIController.instance.virtualCam.ShakeCancel();
            col.enabled = false;
            animator.enabled = false;
            navMeshAgent.enabled = false;
            IsKinematic(false);
            Vector3 dir = transform.position - killer.position;
            for (int i = 0; i < rbs.Length; i++)
            {
                rbs[i].AddForce(new Vector3(dir.x, dir.y + 0.5f, dir.z) * 1.5f, ForceMode.Impulse);
            }
        }

        public void IsKinematic(bool isKinematic)
        {
            for (int i = 0; i < rbs.Length; i++)
            {
                rbs[i].isKinematic = isKinematic;
            }
        }

        void ChangeLookAt()
        {
            lookAt = gameObject;
        }

        public void PlayBlood()
        {
            blood.Play();
        }

        public void SetMaterial(Material material)
        {
            if (material == null)
            {
                amountSmoke--;
            }
            else
            {
                meshRenderer.material = material;
                if (weapon != null)
                {
                    weapon.meshRenderer.material = material;
                    weapon.outline.enabled = false;
                }
                outline.enabled = false;
                amountSmoke++;
            }
            if (amountSmoke == 0)
            {
                meshRenderer.material = defaultMaterial;
                if (weapon != null)
                {
                    weapon.meshRenderer.material = weapon.defaultMaterial;
                    weapon.outline.enabled = true;
                }
                outline.enabled = true;
            }
        }

        public void ResetPlayer()
        {
            navMeshAgent.angularSpeed = 0;
            IsKinematic(true);
            animator.enabled = true;
            isKilling = false;
            transform.rotation = Quaternion.identity;
            col.enabled = true;
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }
    }
}
