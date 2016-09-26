using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Settings : MonoBehaviour
    {
        public LevelManager levelManager;
        public SoundManager soundManager;
        public GameObject volumeSlider;
        public Slider slider;
        public Text volumeLabel;
        // Use this for initialization
        void Start()
        {
            //levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
            //soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();

            levelManager = GameObject.FindObjectOfType<LevelManager>();
            soundManager = GameObject.FindObjectOfType<SoundManager>();

            volumeSlider = GameObject.Find("VolumeSlider");
            slider = volumeSlider.GetComponent<Slider>();
            volumeLabel = GameObject.Find("VolumeDisplayLbl").GetComponent<Text>();

            slider.value = SettingsManager.GetVolume();
            UpdateVolume();
        }

        public void Back_Click()
        {
            if (levelManager.GameInProgress)
            {
                BackToGame();
            }
            else
            {
                BackToMainMenu();
            }
        }

        public void BackToGame()
        {
            levelManager.LoadLevel("Level 01");
        }

        public void BackToMainMenu()
        {
            levelManager.LoadLevel("MainMenu");
        }

        public void UpdateVolume()
        {
            SettingsManager.SetVolume(slider.value);
            volumeLabel.text = slider.value.ToString();
            //soundManager.GetComponent<AudioSource>().volume = SettingsManager.GetVolume() / 100;
            soundManager.SetVolume(SettingsManager.GetVolume() / 100);
        }
    }
}
