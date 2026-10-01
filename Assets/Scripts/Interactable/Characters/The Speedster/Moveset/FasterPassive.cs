using ForeverFight.GameMechanics.Movement;
using ForeverFight.Interactable.Characters;

// The Speedster's 3 extra movement AP a turn. All the pool logic lives in MovementPassiveAp
// so other characters can get passive movement AP the same way.
public class FasterPassive : MovementPassiveAp
{
    protected override Character.Identity OwnerIdentity => Character.Identity.Speedster;

    protected override string DefaultPassiveName => "Faster";

    protected override string DefaultPassiveDescription => "You get 3 extra movement points each turn";
}
