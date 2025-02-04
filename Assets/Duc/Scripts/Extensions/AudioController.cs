using DG.Tweening;
using UnityEngine;

namespace Duc
{
    public class AudioController : MonoBehaviour
    {
        public static AudioController instance;

        [Header("Audio Souce")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource soundSource;

        [Header("Audio Clip")]
        [SerializeField] private AudioClip[] menu;
        [SerializeField] private AudioClip[] inGame;
        [SerializeField] private AudioClip coinFly;
        [SerializeField] private AudioClip gemFly;
        [SerializeField] private AudioClip buttonClick;
        [SerializeField] private AudioClip firework;
        [SerializeField] private AudioClip win;
        [SerializeField] private AudioClip lose;

        [Header("Gameplay")]
        [SerializeField] private AudioClip pickUp;
        [SerializeField] private AudioClip drop;
        [SerializeField] private AudioClip merge;
        [SerializeField] private AudioClip boosterAxe;
        [SerializeField] private AudioClip boosterHammer;
        [SerializeField] private AudioClip boosterFreeze;

        private void Awake()
        {
            if (instance == null) instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            SetMuteMusic(false);
            SetMuteSounds();
        }

        private bool isMuteMusic;
        public void SetMuteMusic(bool smooth)
        {
            isMuteMusic = Manager.instance.IsMuteMusic;
            if (!smooth)
            {
                musicSource.mute = isMuteMusic;
                musicSource.volume = isMuteMusic ? 0f : 0.3f;
                return;
            }

            musicSource.DOFade(isMuteMusic ? 0f : 0.3f, 0.2f).OnComplete(() =>
            {
                musicSource.mute = isMuteMusic;
            }).SetUpdate(true);
        }

        public void SetMuteSounds()
        {
            if (Manager.instance.IsMuteSound)
            {
                soundSource.mute = true;
                return;
            }

            soundSource.mute = false;
        }

        public void PlaySoundButtonClick()
        {
            PlaySFX(buttonClick);
            Vibration.Vibrate(28);
        }

        public void PlayMenuMusic()
        {
            if (menu.Length > 0)
            {
                AudioClip audioClip = menu[Random.Range(0, menu.Length)];
                if (audioClip != null)
                {
                    musicSource.clip = audioClip;
                    musicSource.loop = true;
                    musicSource.Play();
                }
            }
        }

        public void StopMenuMusic()
        {
            musicSource.Stop();
        }

        public void PlayInGameMusic()
        {
            if (inGame.Length > 0)
            {
                AudioClip audioClip = inGame[Random.Range(0, inGame.Length)];

                if (audioClip != null)
                {
                    musicSource.clip = audioClip;
                    musicSource.Play();
                }
            }
        }

        public void PlaySFX(AudioClip audioClip)
        {
            if (Manager.instance.IsMuteSound) return;

            if (audioClip != null)
            {
                soundSource.PlayOneShot(audioClip);
            }
        }

        public void PlaySoundCoinFly()
        {
            PlaySFX(coinFly);
        }
        
        public void PlaySoundGemFly()
        {
            PlaySFX(gemFly);
        }
        
        public void PlaySoundFirework()
        {
            PlaySFX(firework);
        }
        
        public void PlaySoundWin()
        {
            PlaySFX(win);
        }

        public void PlaySoundLose()
        {
            PlaySFX(lose);
        }
        
        public void PlaySoundPickUp()
        {
            PlaySFX(pickUp);
            Vibration.Vibrate(25);
        }

        public void PlaySoundDrop()
        {
            PlaySFX(drop);
            Vibration.Vibrate(25);
        }
        
        public void PlaySoundMerge()
        {
            PlaySFX(merge);
            Vibration.Vibrate(25);
        }
        
        public void PlaySoundBoosterAxe()
        {
            PlaySFX(boosterAxe);
        }
        
        public void PlaySoundBoosterHammer()
        {
            PlaySFX(boosterHammer);
        }
        
        public void PlaySoundBoosterFreeze()
        {
            PlaySFX(boosterFreeze);
        }
    }
}