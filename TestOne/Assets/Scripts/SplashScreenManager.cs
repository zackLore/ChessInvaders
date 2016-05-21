using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class SplashScreenManager : MonoBehaviour
    {
        public float LoadTime = 2f;
        void Start()
        {
            Invoke("LoadGame", LoadTime);
        }

        void LoadGame()
        {
            SceneManager.LoadScene(1);
        }
    }
}
