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

public class PlayerHitSpikeEvent : GameEvent
{

}

public class ShakeCameraEvent : GameEvent
{

}

public class StartGameEvent : GameEvent
{

}

public class LevelInitiatedEvent : GameEvent
{

}

public class ResetLevelEvent : GameEvent
{

}

public class KeyHitSpikeEvent : GameEvent
{

}

public class ActivateButtonEffect : GameEvent
{
    public RecorderButtonType recorderButtonType;
    public int groupID;

    public ActivateButtonEffect(RecorderButtonType recorderButtonType, int groupID)
    {
        this.recorderButtonType = recorderButtonType;
        this.groupID = groupID;
    }
}