using ForverFight.Networking;
using UnityEngine;
using UnityEngine.UI;

public class PassDynamicCombatReferences : MonoBehaviour
{
    public void PassOpponentsHealthSlider(Slider value)
    {
        LocalStoredNetworkData.opponentHealthSlider = value;
    }

    public void PassConfirmAttackButton(Button value)
    {
        LocalStoredNetworkData.localPlayerAttackConfirmButton = value;
    }
}
