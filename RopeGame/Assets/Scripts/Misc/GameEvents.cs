using Core;
using UnityEngine;

public class ObjectReachedEndEvent : GameEvent
{
    public ObjectType objectType;
    public GameObject obj;

    public ObjectReachedEndEvent(ObjectType objectType, GameObject obj)
    {
        this.objectType = objectType;
        this.obj = obj;
    }
}

public class GameStateChangedEvent : GameEvent
{
    public GameState gameState;

    public GameStateChangedEvent(GameState gameState)
    {
        this.gameState = gameState;
    }
}

public class GameStateCompleteEvent : GameEvent
{
    public GameState gameState;

    public GameStateCompleteEvent(GameState gameState)
    {
        this.gameState = gameState;
    }
}