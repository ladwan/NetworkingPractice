using ForeverFight.GameMechanics.Movement;

namespace ForeverFight.Interactable.PlayerInputInteractions
{
    /// <summary>
    /// Sits on the guide line's endpoint grab handle (built by MovementGuideLine).
    /// Clicking a released plan's endpoint resumes the movement drag from where it
    /// left off; releasing ends it again, same as dragging from the character.
    /// </summary>
    public class MoveEndpointInteractable : BaseInteractable
    {
        protected override void OnClicked()
        {
            if (MovementPlanner.Instance != null)
            {
                MovementPlanner.Instance.ResumeDrag();
            }
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
