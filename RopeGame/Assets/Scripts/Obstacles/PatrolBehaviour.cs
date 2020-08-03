using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolBehaviour : MonoBehaviour
{
    [SerializeField] private Vector3[] waypoints;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float waitTime;

    private int currentPathIndex = 0;
    private int targetPathIndex = 0;

    private bool startWaiting;
    private DateTime waitTimeStamp;
    
    private Vector2 targetPosition;
    private int moveDirection = 1;

    #region Base methods

    private void Start()
    {
        currentPathIndex = 0;
        targetPathIndex = 0;

        moveDirection = 1;

        startWaiting = false;
        waitTimeStamp = DateTime.Now;

        transform.position = waypoints[currentPathIndex];
        targetPosition = waypoints[targetPathIndex];
    }

    private void Update()
    {
        if (startWaiting)
        {
            if ((DateTime.Now - waitTimeStamp).TotalMilliseconds >= waitTime * 1000)
            {
                startWaiting = false;
            }
        }
        else
        {
            if (currentPathIndex != targetPathIndex)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }

            if (Vector2.Distance(transform.position, targetPosition) == 0)
            {
                if (moveDirection == 1)
                {
                    currentPathIndex++;
                    targetPathIndex++;
                }
                else
                {
                    currentPathIndex--;
                    targetPathIndex--;
                }

                if (currentPathIndex == waypoints.Length - 1)
                {
                    moveDirection *= -1;

                    targetPathIndex = currentPathIndex - 1;
                }
                else if (currentPathIndex == 0)
                {
                    moveDirection *= -1;

                    targetPathIndex = currentPathIndex + 1;
                }

                targetPosition = waypoints[targetPathIndex];

                if (waitTime > 0)
                {
                    startWaiting = true;
                    waitTimeStamp = DateTime.Now;
                }
            }
        }
    }

    #endregion
}
