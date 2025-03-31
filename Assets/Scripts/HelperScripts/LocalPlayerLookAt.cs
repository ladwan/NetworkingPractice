using System.Collections;
using System.Collections.Generic;
using ForeverFight.FlowControl;
using UnityEngine;

namespace ForeverFight.HelperScripts
{
    public class LocalPlayerLookAt : BasePlayerLookAt
    {
        protected void Awake()
        {
            IsPlayer1 = ClientInfo.playerNumber == 1 ? true : false;
            StartCoroutine(WaitForTransformReferences());
            //make an iEnum that will keep trying this until its not null
        }

        private IEnumerator WaitForTransformReferences()
        {
            yield return new WaitUntil(() => Player2Transform != null);
            Target = IsPlayer1 ? Player2Transform : Player1Transform;
        }

        private void LateUpdate()
        {
            if (IsPlayerTurnAndTargetValid(Target))
            {
                RotateTowardsTargetAndDisable(this, Target.position, transform);
            }
            else if (!PlayerTurnManager.Instance.IsLocalPlayersTurn && Target != null)
            {
                RotateTowardsTarget(this, Target.position, transform);
            }
        }
    }
}
