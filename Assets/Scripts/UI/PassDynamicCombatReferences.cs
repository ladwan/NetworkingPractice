using ForeverFight.Networking;
using UnityEngine;
using UnityEngine.UI;

public class PassDynamicCombatReferences : MonoBehaviour
{
    public void PassLocalPlayerHealthSlider(Slider value)
    {
        LocalStoredNetworkData.localPlayerHealthSlider = value;
    }

    public void PassOpponentsHealthSlider(Slider value)
    {
        LocalStoredNetworkData.opponentHealthSlider = value;
    }

    public void PassConfirmAttackButton(Button value)
    {
        LocalStoredNetworkData.localPlayerAttackConfirmButton = value;
    }
}
