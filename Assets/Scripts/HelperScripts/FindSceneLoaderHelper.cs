using ForeverFight.FlowControl;
using ForeverFight.HelperScripts;
using UnityEngine;

namespace HelperScripts
{
    public class FindSceneLoaderHelper : MonoBehaviour
    {
        public void FindSceneLoaderAndLoadScene(string sceneName)
        {
            if (!SafetyNet.IsValid(SceneLoader.Instance, "SceneLoader Instance"))
            {
                return;
            }
            
            SceneLoader.Instance.LoadScene(sceneName);
        }
    }
}
