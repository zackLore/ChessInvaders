using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class SoundManager : MonoBehaviour
    {
        public AudioClip[] Music;
        public AudioClip[] Sounds;
        private AudioSource MusicManager;

        void Start()
        {
            DontDestroyOnLoad(this);
            MusicManager = GetComponent<AudioSource>();
            MusicManager.clip = Music[0];
            MusicManager.loop = true;

            if (SettingsManager.GetVolume() >= 0)
            {
                MusicManager.volume = SettingsManager.GetVolume() / 100;
            }
            else
            {
                MusicManager.volume = .50f;
            }

            MusicManager.Play();
        }

        public void PlayMusic(int index)
        {
            MusicManager.Stop();
            MusicManager.clip = Music[index];
            MusicManager.Play();
        }

        public void PlayMusic(string clipName)
        {

        }

        public void PlaySound(int index)
        {
            MusicManager.Stop();
            MusicManager.clip = Sounds[index];
            MusicManager.Play();
        }

        public void PlaySound(string clipName)
        {

        }
    }
}
