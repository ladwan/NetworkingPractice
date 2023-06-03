using ForverFight.Interactable.Abilities;
using ForverFight.Networking;
using UnityEngine;

public class CharacterDataManager : MonoBehaviour
{
    [SerializeField]
    private SelectAbilityToCast selectAbilityToCast = null;

    public void ToggleAbilityRadius(bool value)
    {
        selectAbilityToCast = LocalStoredNetworkData.localPlayerSelectAbilityToCast;
        if (selectAbilityToCast)
        {
            //selectAbilityToCast.ToggleAbilityRadius(value);
        }
    }
}