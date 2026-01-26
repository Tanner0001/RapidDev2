
public enum Faction
{
    CoastGuard,
    Civilian,
    Smuggler,
    HostileMilitary
}

public enum UnitState
{
    Idle,
    Moving,
    Patrolling,  // New state for following a lane
    Scanning     // Stationary, performing action
}

public enum ThreatLevel
{
    Unknown, // Grey
    Green,   // Clear
    Red      // Hostile
}
