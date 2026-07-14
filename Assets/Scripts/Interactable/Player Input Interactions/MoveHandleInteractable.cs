using UnityEngine;
using ForeverFight.GameMechanics.Movement;

namespace ForeverFight.Interactable.PlayerInputInteractions
{
    /// <summary>
    /// Sits on each player spawn's drag collider (layer "Drag Movement"), replacing the
    /// old DragInteractable token. Clicking your own character begins a movement drag;
    /// releasing ends it. All validity guards (turn, UI state, AP) live in the planner.
    /// </summary>
    public class MoveHandleInteractable : BaseInteractable
    {
        private GameObject owningSpawn = null;

        public GameObject OwningSpawn { get => owningSpawn; set => owningSpawn = value; }


        protected override void OnClicked()
        {
            if (MovementPlanner.Instance == null || PlayerSpawnManager.Instance == null)
            {
                return;
            }

            if (owningSpawn != PlayerSpawnManager.Instance.LocalPlayerSpawn)
            {
                return; // Only the local player's character is draggable.
            }

            MovementPlanner.Instance.BeginDrag();
        }

        protected override void OnUnclicked()
        {
            if (MovementPlanner.Instance != null)
            {
                MovementPlanner.Instance.EndDrag();
            }
        }
    }
}
