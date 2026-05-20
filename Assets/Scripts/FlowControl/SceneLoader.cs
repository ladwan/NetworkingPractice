using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ForeverFight.FlowControl
{
    public class SceneLoader : MonoBehaviour
    {
        private static SceneLoader instance = null;

        public static SceneLoader Instance => instance;

        public event Action<string> OnSceneLoaded;

        protected void Awake()
        {
            if (instance == null)
            {
                instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.Log("More Than 1 SceneLoader detected, Destroying self...");
                Destroy(gameObject);
            }
        }

        public void LoadScene(string scene)
        {
            StartCoroutine(LoadSceneAsync(scene));
        }

        private IEnumerator LoadSceneAsync(string scene)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(scene);
            yield return op;
            OnSceneLoaded?.Invoke(scene);
        }
    }
}
