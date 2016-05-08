using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class SoundManager : MonoBehaviour
    {
        public AudioClip[] Music;
        public AudioClip[] Sounds;
        private AudioSource Manager;

        void Start()
        {
            DontDestroyOnLoad(this);
            Manager = GetComponent<AudioSource>();
            Manager.clip = Music[0];

            if (SettingsManager.GetVolume() >= 0)
            {
                Manager.volume = SettingsManager.GetVolume() / 100;
            }
            else
            {
                Manager.volume = .50f;
            }

            Manager.Play();
        }

        public void PlayMusic(int index)
        {
            Manager.Stop();
            Manager.clip = Music[index];
            Manager.Play();
        }

        public void PlayMusic(string clipName)
        {

        }

        public void PlaySound(int index)
        {
            Manager.Stop();
            Manager.clip = Sounds[index];
            Manager.Play();
        }

        public void PlaySound(string clipName)
        {

        }
    }
}
