using Core;

public class PlayerStateChangedEvent : GameEvent
{
    public PlayerState playerState;

    public PlayerStateChangedEvent(PlayerState playerState)
    {
        this.playerState = playerState;
    }
}

public class PlayerReachedEndEvent : GameEvent
{

}

public class StartGameEvent : GameEvent
{

}
