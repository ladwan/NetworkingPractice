using ForverFight.Networking;
using UnityEngine;
using UnityEngine.UI;

public class PassDynamicCombatReferences : MonoBehaviour
{
    [Header("Player 1")]
    [SerializeField]
    private Button player1AttackConfirmButton = null;

    [Header("Player 2")]
    [SerializeField]
    private Button player2AttackConfirmButton = null;


    protected void OnEnable()
    {
        if (ClientInfo.playerNumber == 1)
        {
            LocalStoredNetworkData.localPlayerAttackConfirmButton = player1AttackConfirmButton;
        }
        if (ClientInfo.playerNumber == 2)
        {
            LocalStoredNetworkData.localPlayerAttackConfirmButton = player2AttackConfirmButton;
        }
    }
}
