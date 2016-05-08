using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class LevelManager : MonoBehaviour
    {
        public string[] Scenes;
        public int CurrentIndex;
        public bool GameInProgress = false;

        void Start()
        {
            DontDestroyOnLoad(this);
            CurrentIndex = 0;
        }

        private void Load()
        {
            SceneManager.LoadScene(Scenes[CurrentIndex].ToString());
        }

        public void LoadLevel(int index)
        {
            if (index >= 0 && index < Scenes.Length)
            {
                CurrentIndex = index;
                Load();
            }
        }

        public void LoadLevel(string sceneName)
        {
            //Debug.Log(sceneName);
            for (int i = 0; i < Scenes.Length; i++)
            {
                if (sceneName == Scenes[i])
                {
                    //Debug.Log(i);
                    CurrentIndex = i;
                    SceneManager.LoadScene(CurrentIndex);
                    break;
                }
            }
        }

        public void LoadNextLevel()
        {
            if (CurrentIndex >= 0 && CurrentIndex < Scenes.Length - 1)
            {
                CurrentIndex++;
            }
            else if (CurrentIndex >= Scenes.Length - 1 || CurrentIndex < 0)
            {
                CurrentIndex = 0;
            }

            Load();
        }
        
    }
}
