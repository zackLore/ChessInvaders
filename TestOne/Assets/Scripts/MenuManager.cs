using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class MenuManager : MonoBehaviour
    {
        public GameObject Levels;
        public GameObject Sounds;
        LevelManager levelManager = null;
        SoundManager soundManager = null;

        void Start()
        {
            LevelManager lm = GameObject.FindObjectOfType<LevelManager>();
            SoundManager sm = GameObject.FindObjectOfType<SoundManager>();

            if (lm == null)
            {
                Levels = ((GameObject)Instantiate(Resources.Load(@"Prefabs/LevelManager")));
                levelManager = Levels.GetComponent<LevelManager>();
                GameObject.DontDestroyOnLoad(levelManager);
            }
            else
            {
                levelManager = lm;
            }

            if (sm == null)
            {
                Sounds = ((GameObject)Instantiate(Resources.Load(@"Prefabs/SoundManager")));
                soundManager = Sounds.GetComponent<SoundManager>();
                GameObject.DontDestroyOnLoad(soundManager);
            }
            else
            {
                soundManager = sm;
            }

            //if (levelManager == null)
            //{
            //    //levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
            //    Levels = ((GameObject)Instantiate(Resources.Load(@"Prefabs/LevelManager")));
            //    //levelManager = (LevelManager)((GameObject)Instantiate(Resources.Load(@"Prefabs/Player"))).GetComponent<LevelManager>();
            //    levelManager = Levels.GetComponent<LevelManager>();
            //    GameObject.DontDestroyOnLoad(levelManager);
                
            //}
            //if (soundManager == null)
            //{
            //    //soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
            //    Sounds = ((GameObject)Instantiate(Resources.Load(@"Prefabs/SoundManager")));
            //    //soundManager = (SoundManager)((GameObject)Instantiate(Resources.Load(@"Prefabs/Player"))).GetComponent<SoundManager>();
            //    soundManager = Sounds.GetComponent<SoundManager>();
            //    GameObject.DontDestroyOnLoad(soundManager);
            //}
            //Debug.Log(levelManager);
        }

        public void Play_Click()
        { 
            levelManager.LoadLevel("Level 01");
            soundManager.PlayMusic(levelManager.CurrentIndex);
        }

        public void MainMenu_Click()
        {
            soundManager.PlayMusic(0);
            levelManager.LoadLevel("MainMenu");
        }

        public void Exit_Click()
        {
            Application.Quit();
        }

        public void Back_Click()
        {
            gameObject.SetActive(false);
        }
    }
}
