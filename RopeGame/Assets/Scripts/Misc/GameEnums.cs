public enum ObjectType
{
    None,
    Player
}

public enum PlayerLocation
{
    None,
    Grounded,
    InAir
}

public enum PlayerState
{
    None,
    IdleMove,
    Jump,
    LatchUnderProcess,
    Latched,
    ReleasedFromLatch,
    MoveByBand,
}