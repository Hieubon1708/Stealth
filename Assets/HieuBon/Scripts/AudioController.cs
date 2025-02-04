using Duc;
using TigerForge;
using UnityEngine;

namespace Hunter
{
    public class AudioController : MonoBehaviour
    {
        public static AudioController instance;

        public AudioSource srcMusic;
        public AudioSource srcSound;
        public AudioSource srcAlert;
        public AudioSource srcTurrel;
        public AudioSource srcElevator;

        public AudioClip button;
        public AudioClip glassBrocken;
        public AudioClip laser;
        public AudioClip blastBarrel;
        public AudioClip shotGun;
        public AudioClip laserGun;
        public AudioClip ak47Gun;
        public AudioClip venomAttack;
        public AudioClip pistol;
        public AudioClip objectBrocken;
        public AudioClip boomerang;
        public AudioClip alert;
        public AudioClip enemyDie;
        public AudioClip playerDie;
        public AudioClip enemyDetect;
        public AudioClip enemyDamage;
        public AudioClip coin;
        public AudioClip openDoor;
        public AudioClip cut;
        public AudioClip pickUp;
        public AudioClip flowerPotBrocken;
        public AudioClip doorOpen;
        public AudioClip win;
        public AudioClip lose;

        private void Awake()
        {
            instance = this;
        }

        public void Start()
        {
            EventManager.StartListening(EventVariables.SetMusic, IsPlayMusic);
        }

        public void IsPlayMusic()
        {
            srcMusic.mute = Manager.instance.IsMuteMusic;
        }

        public void PlaySoundNVibrate(AudioClip audioClip, int value)
        {
            if (!Manager.instance.IsMuteSound) srcSound.PlayOneShot(audioClip);
            if (value != 0) Vibration.Vibrate(value);
        }

        public void PlayAlert()
        {
            if (!Manager.instance.IsMuteSound) srcAlert.Play();
        }

        public void StopAlert()
        {
            srcAlert.Stop();
        }

        public void PlayTurrel()
        {
            if (!Manager.instance.IsMuteSound) srcTurrel.Play();
        }

        public void StopTurrel()
        {
            srcTurrel.Stop();
        }
        
        public void PlayElevator()
        {
            if (!Manager.instance.IsMuteSound) srcElevator.Play();
        }

        public void StopElevator()
        {
            srcElevator.Stop();
        }

        public void ResetAudio()
        {
            StopAlert();
        }
    }
}
