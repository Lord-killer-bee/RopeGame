using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GameState currentGameState;

    private void OnEnable()
    {
        GameEventManager.Instance.AddListener<ObjectReachedEndEvent>(OnObjectCompletedLevel);
        GameEventManager.Instance.AddListener<GameStateCompleteEvent>(OnGameStateCompleted);
    }

    private void OnDisable()
    {
        GameEventManager.Instance.RemoveListener<ObjectReachedEndEvent>(OnObjectCompletedLevel);
        GameEventManager.Instance.RemoveListener<GameStateCompleteEvent>(OnGameStateCompleted);
    }

    private void Start()
    {
        SetState(GameState.CountDown);
    }

    #region States related

    void SetState(GameState state)
    {
        if (currentGameState != state)
        {
            currentGameState = state;

            switch (currentGameState)
            {
                case GameState.CountDown:
                    break;
                case GameState.Match:
                    break;
                case GameState.EndSequence:
                    break;
            }

            GameEventManager.Instance.TriggerSyncEvent(new GameStateChangedEvent(currentGameState));
        }
    }

    #endregion

    #region Event listeners

    private void OnObjectCompletedLevel(ObjectReachedEndEvent e)
    {
        SetState(GameState.EndSequence);
    }

    private void OnGameStateCompleted(GameStateCompleteEvent e)
    {
        switch (e.gameState)
        {
            case GameState.CountDown:
                SetState(GameState.Match);
                break;
            case GameState.Match:
                SetState(GameState.EndSequence);
                break;
            case GameState.EndSequence:
                break;
        }
    }

    #endregion
}
