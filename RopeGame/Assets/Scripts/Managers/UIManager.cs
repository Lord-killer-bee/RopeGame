using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject countDownObject;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject replayButton;

    private void OnEnable()
    {
        GameEventManager.Instance.AddListener<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnDisable()
    {
        GameEventManager.Instance.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        switch (e.gameState)
        {
            case GameState.CountDown:
                countDownObject.SetActive(true);

                Invoke("EndCountDown", 3f);
                break;
            case GameState.Match:
                break;
            case GameState.EndSequence:
                Invoke("ShowWinner", 1f);
                break;
        }
    }

    void EndCountDown()
    {
        countDownObject.SetActive(false);
        GameEventManager.Instance.TriggerSyncEvent(new GameStateCompleteEvent(GameState.CountDown));
    }

    void ShowWinner()
    {
        winScreen.SetActive(true);
        Invoke("EnableReplayButton", 1f);
    }

    void EnableReplayButton()
    {
        replayButton.SetActive(true);
    }

    public void ReplayLevel()
    {
        SceneManager.LoadScene(0);
    }
}
