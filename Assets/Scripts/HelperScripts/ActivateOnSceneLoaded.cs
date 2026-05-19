using System.Collections.Generic;
using ForeverFight.FlowControl;
using UnityEngine;

namespace HelperScripts
{
    public class ActivateOnSceneLoaded : MonoBehaviour
    {
        [SerializeField] private GameObject target;
        [SerializeField] private List<string> sceneNames;

        private void Start()
        {
            if (SceneLoader.Instance != null)
                SceneLoader.Instance.OnSceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            if (SceneLoader.Instance != null)
                SceneLoader.Instance.OnSceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(string sceneName)
        {
            if (sceneNames.Contains(sceneName))
                return;

            if (!target.activeSelf)
                target.SetActive(true);
        }
    }
}
