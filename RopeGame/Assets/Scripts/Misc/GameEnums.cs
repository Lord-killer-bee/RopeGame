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
    CastingBand,
    MoveByBand,
    StuckOnWall,
    JumpFromWall
}

public enum PlayerJumpState
{
    None,
    JumpUp,
    ReachedPeak,
    FallingDown,
    HitGround
}

public enum BoulderType
{
    None,
    Unmovable,
    Movable,
    Destructible,
}