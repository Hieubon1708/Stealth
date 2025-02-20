using UnityEngine;

namespace Hunter
{
    public class Cage : MonoBehaviour
    {
        public ParticleSystem bum;
        public MeshCollider[] cols;
        public Rigidbody[] rbs;
        public Player player;
        public AnimationClip hostageAni;
        AnimationClip idle;
        public GameObject old;

        void Awake()
        {
            AnimatorOverrideController overrideController = new AnimatorOverrideController(player.animator.runtimeAnimatorController);
            idle = overrideController["Idle"];
            overrideController["Idle"] = hostageAni;
            player.animator.runtimeAnimatorController = overrideController;
        }

        public virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                bum.Play();
                player.col.enabled = true;
                player.outline.enabled = true;
                AnimatorOverrideController overrideController = new AnimatorOverrideController(player.animator.runtimeAnimatorController);
                overrideController["Idle"] = idle;
                player.animator.runtimeAnimatorController = overrideController;
                player.ResetPlayer();
                for (int i = 0; i < rbs.Length; i++)
                {
                    rbs[i].isKinematic = false;
                }
                GameController.instance.poppies.Add(player);
                AIManager.Instance.Init();
                AudioController.instance.PlaySoundNVibrate(AudioController.instance.objectBrocken, 0);
                old.SetActive(false);
                Invoke(nameof(Drop), 3.5f);
            }
        }

        public void Drop()
        {
            for (int i = 0; i < cols.Length; i++)
            {
                cols[i].enabled = false;
            }
            Invoke(nameof(Hide), 3.5f);
        }

        void Hide()
        {
            for (int i = 0; i < rbs.Length; i++)
            {
                rbs[i].gameObject.SetActive(false);
            }
        }
    }
}
