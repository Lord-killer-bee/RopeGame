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

public enum EntityActionType
{
    None,
    Play,
    Pause,
    Rewind,
    FastForward,
    Stop
}

public enum RecorderButtonType
{
    None,
    Play,
    Pause,
    Stop,
    Rewind,
    FastForward,
    Next,
    Previous
}