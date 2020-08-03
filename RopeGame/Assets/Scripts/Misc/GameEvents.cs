using Core;

public class PlayerRevolutionCompleteEvent : GameEvent
{

}

public class PlayerStateChangedEvent : GameEvent
{
    public PlayerState playerState;

    public PlayerStateChangedEvent(PlayerState playerState)
    {
        this.playerState = playerState;
    }
}

public class EnemyReachedTargetEvent : GameEvent
{

}


public class PlayerReachedEndEvent : GameEvent
{

}

public class StartGameEvent : GameEvent
{

}

public class InitiateLevelEvent : GameEvent
{

}