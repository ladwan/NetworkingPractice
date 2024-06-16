using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Networking;

namespace ForeverFight.HelperScripts.Animation
{
    public class StartTimerOnAnimEnd : MonoBehaviour
    {
        public void AnimEnd() //Animation events will call this to restart the timer when the animation is done !
        {
            LocalStoredNetworkData.GetCountdownTimerScript().TellNetworkToToggleTimer();
        }
    }
}

