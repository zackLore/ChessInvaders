using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class SoundManager : MonoBehaviour
    {
        public AudioClip[] Music;
        public AudioClip[] Sounds;
        public AudioSource MusicManager_;
        public AudioSource SoundManager_;

        void Start()
        {
            DontDestroyOnLoad(this);
            var audioSources = GetComponents<AudioSource>();
            MusicManager_ = audioSources[0];
            
            MusicManager_.clip = Music[0];
            MusicManager_.loop = true;

            if (SettingsManager.GetVolume() >= 0)
            {
                MusicManager_.volume = SettingsManager.GetVolume() / 100;
            }
            else
            {
                MusicManager_.volume = .50f;
            }

            MusicManager_.Play();
        }

        public void PlayMusic(int index)
        {
            MusicManager_.Stop();
            MusicManager_.clip = Music[index];
            MusicManager_.Play();
        }

        public void PlayMusic(string clipName)
        {

        }

        public void PlaySound(int index)
        {
            MusicManager_.Stop();
            MusicManager_.clip = Sounds[index];
            MusicManager_.Play();
        }

        public void PlaySound(GameObject piece, string clipName, bool loopSound = false)
        {
            SoundManager_ = piece.GetComponent<AudioSource>();
            Debug.Log(piece);
            if (SoundManager_ == null){ return; }

            Debug.Log(clipName + "...");

            foreach (var sound in Sounds)
            {
                if (sound.name == clipName)
                {
                    Debug.Log("Play Sound: " + clipName);
                    SoundManager_.Stop();
                    SoundManager_.clip = sound;
                    SoundManager_.loop = loopSound;
                    SoundManager_.Play();
                }
            }
        }

        public void SetVolume(float volume)
        {
            if (MusicManager_ != null)
            {
                MusicManager_.volume = volume;
            }
            if (SoundManager_ != null)
            {
                SoundManager_.volume = volume;
            }
        }

        public void StopSound()
        {
            if (SoundManager_ != null)
            {
                SoundManager_.Stop();
            }
        }
    }
}
