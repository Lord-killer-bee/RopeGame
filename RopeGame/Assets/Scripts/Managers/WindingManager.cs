using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using System;

public class WindingManager : MonoBehaviour
{
    [SerializeField] private GameObject windingTarget;
    [SerializeField] private GameObject windingEnemy;

    [SerializeField] private float windingSteps;
    [SerializeField] private float windingStepDistance;

    [SerializeField] private float initialEnemyMoveSpeed;
    [SerializeField] private float enemySpeedMultiplier;

    private float currentEnemySpeed;
    private PlayerState currentPlayerState;
    private Vector3 initialPosition;
    private Vector3 enemyDestinationPosition = new Vector3(0, -9.5f, 0);
    private int currentStep;
    private bool enemyActive = false;

    DateTime startTime;

    private void OnEnable()
    {
        GameEventManager.Instance.AddListener<PlayerRevolutionCompleteEvent>(OnRevolutionComplete);
        GameEventManager.Instance.AddListener<PlayerStateChangedEvent>(OnPlayerStateChanged);
        GameEventManager.Instance.AddListener<InitiateLevelEvent>(OnInitiateLevel);
        GameEventManager.Instance.AddListener<PlayerReachedEndEvent>(OnPlayerReachedEnd);
    }

    private void OnDisable()
    {
        GameEventManager.Instance.RemoveListener<PlayerRevolutionCompleteEvent>(OnRevolutionComplete);
        GameEventManager.Instance.RemoveListener<PlayerStateChangedEvent>(OnPlayerStateChanged);
        GameEventManager.Instance.RemoveListener<InitiateLevelEvent>(OnInitiateLevel);
        GameEventManager.Instance.RemoveListener<PlayerReachedEndEvent>(OnPlayerReachedEnd);
    }

    private void Update()
    {
        UpdateWinding();
    }

    private void UpdateWinding()
    {
        switch (currentPlayerState)
        {
            case PlayerState.None:
            case PlayerState.IdleMove:
            case PlayerState.Jump:
            case PlayerState.LatchUnderProcess:
            case PlayerState.ReleasedFromLatch:
                if(enemyActive)
                    windingEnemy.transform.localPosition = Vector3.MoveTowards(windingEnemy.transform.localPosition, enemyDestinationPosition, currentEnemySpeed * Time.deltaTime);
                break;
            case PlayerState.Latched:

                break;

        }
    }

    public void EnemyReachedTarget()
    {
        enemyActive = false;
        GameEventManager.Instance.TriggerSyncEvent(new EnemyReachedTargetEvent());

        Debug.Log("Current step : " + currentStep + ", Time : " + (DateTime.Now - startTime).TotalMilliseconds / 1000);
    }

    #region Event listeners

    private void OnInitiateLevel(InitiateLevelEvent e)
    {
        currentStep = 0;
        initialPosition = windingTarget.transform.position;
        currentEnemySpeed = initialEnemyMoveSpeed;
        enemyActive = true;

        startTime = DateTime.Now;
    }

    private void OnRevolutionComplete(PlayerRevolutionCompleteEvent e)
    {
        if (currentStep < windingSteps)
        {
            currentStep++;

            windingTarget.transform.position = new Vector3(initialPosition.x, initialPosition.y - currentStep * windingStepDistance, initialPosition.z);

            currentEnemySpeed += enemySpeedMultiplier;
        }
    }

    private void OnPlayerStateChanged(PlayerStateChangedEvent e)
    {
        currentPlayerState = e.playerState;
    }

    private void OnPlayerReachedEnd(PlayerReachedEndEvent e)
    {
        enemyActive = false;
    }

    #endregion
}
