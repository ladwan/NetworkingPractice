namespace ForeverFight.GameMechanics.Movement
{
    /// <summary>
    /// A movement-only bonus AP pool a character can contribute (e.g. the Speedster's
    /// Faster passive). ApDistanceBank drains this pool before main AP while planning a
    /// move and refunds it last on backtrack; the pool renders on its own distinct light
    /// bar. To give a character passive movement AP: implement this on the passive's
    /// component, point an ApReferenceLists (display type movementPassive) at its light
    /// bar, and register/unregister via ActionPointsManager.Instance.MovementPassiveApProvider.
    /// </summary>
    public interface IMovementPassiveAp
    {
        /// <summary>AP currently left in the pool this turn.</summary>
        int PassiveAp { get; set; }

        /// <summary>Pool size the AP is clamped to (and typically resets to each turn).</summary>
        int MaxPassiveAp { get; }

        /// <summary>The distinct UI light bar this pool blinks, refunds and commits on.</summary>
        ApReferenceLists PassiveApLists { get; }
    }
}
