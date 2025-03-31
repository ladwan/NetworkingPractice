using System.Collections;
using System.Collections.Generic;
using ForeverFight.FlowControl;
using UnityEngine;

namespace ForeverFight.HelperScripts
{
    public class RemotePlayerLookAt : BasePlayerLookAt
    {
        //This is the look at for the remote player, first we need to find out the player number of the owner of this game instance
        //Remember this is NOT for rotating the charather the local player controls, so if the local player is player 1
        //This remote look at's target should be player 1
        protected void Awake()
        {
            IsPlayer1 = ClientInfo.playerNumber == 1 ? true : false;
            StartCoroutine(WaitForTransformReferences());
            //make an iEnum that will keep trying this until its not null
        }

        private IEnumerator WaitForTransformReferences()
        {
            yield return new WaitUntil(() => Player2Transform != null);
            Target = IsPlayer1 ? Player1Transform : Player2Transform;
        }

        private void LateUpdate()
        {
            if (IsPlayerTurnAndTargetValid(Target))
            {
                RotateTowardsTarget(this, Target.position, transform);
            }
            else if (!PlayerTurnManager.Instance.IsLocalPlayersTurn && Target != null)
            {
                RotateTowardsTargetAndDisable(this, Target.position, transform);
            }
        }
    }
}
