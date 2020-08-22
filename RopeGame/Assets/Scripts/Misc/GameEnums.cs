public enum ObjectType
{
    None,
    Player,
    AI
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

public enum AIState
{
    None,
    SearchingForAction,
    ExecutingAction,
    CooldownToSearch
}

public enum AIActionType
{
    None,
    Idle,
    Move,
    Jump,
    Latch,
}

public enum GameState
{
    None,
    CountDown,
    Match,
    EndSequence
}