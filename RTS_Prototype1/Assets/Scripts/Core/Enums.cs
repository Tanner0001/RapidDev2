
public enum Faction
{
    None,
    Player,
    Enemy,
    Civilian
}

public enum UnitState
{
    Idle,
    Moving,
    MovingToAttack, // Added this state
    Patrolling,
    Attacking
}
