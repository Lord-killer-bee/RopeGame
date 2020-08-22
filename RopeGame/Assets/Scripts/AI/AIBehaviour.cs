using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIBehaviour : MonoBehaviour
{
    [SerializeField] private float latchDetectRange = 10f;
    [SerializeField] private float latchRejectRange = 11f;
    [SerializeField] private float latchWaitTime = 1f;
    [SerializeField] private float latchWaitTimeAfterIdle = 2f;
    [SerializeField] private float jumpDetectionRange = 5f;
    [SerializeField] private float moveDistance = 5f;

    [SerializeField] private float searchCooldown = 1f;

    [SerializeField] private Text stateText;
    [SerializeField] private Text actionText;

    private AIState aiState;
    private AIActionType currentAction, lastAction;

    private DateTime cooldownStartTime;
    private DateTime actionSetTime;

    Platformer platformer;

    private int moveDirection = 1;
    private Vector3 moveTarget;

    private Platform targetPlatform;
    private Vector3 jumpTarget;
    private DateTime jumpStartTime;
    private bool jumped;

    private GameObject currentLatchPoint;//TODO: might have to remove it later
    private bool latched;
    private DateTime latchStartTime;

    public void Start()
    {
        aiState = AIState.SearchingForAction;
        currentAction = AIActionType.None;

        platformer = GetComponent<Platformer>();

        SetState(AIState.SearchingForAction);
    }

    void SetState(AIState aiState)
    {
        if (this.aiState != aiState)
            this.aiState = aiState;

        stateText.text = aiState.ToString();

        switch (aiState)
        {
            case AIState.SearchingForAction:
                lastAction = currentAction;
                currentAction = AIActionType.None;
                break;
            case AIState.ExecutingAction:
                break;
            case AIState.CooldownToSearch:
                cooldownStartTime = DateTime.Now;
                break;
        }
    }

    void SetAction(AIActionType actionType)
    {
        if (currentAction != actionType)
        {
            currentAction = actionType;
        }

        actionSetTime = DateTime.Now;

        actionText.text = actionType.ToString();

        switch (currentAction)
        {
            case AIActionType.Idle:

                break;
            case AIActionType.Move:
                //moveDirection = UnityEngine.Random.Range(0, 100) % 2 == 0 ? 1 : -1;

                if (platformer.controller.collisions.left)
                {
                    moveDirection = 1;
                }
                else if (platformer.controller.collisions.right)
                {
                    moveDirection = -1;
                }

                moveTarget = transform.position + new Vector3(moveDistance * moveDirection, 0, 0);
                break;
            case AIActionType.Jump:
                if (Vector3.Distance(transform.position, targetPlatform.GetLeftPoint()) < Vector3.Distance(transform.position, targetPlatform.GetRightPoint()))
                {
                    jumpTarget = targetPlatform.GetLeftPoint();
                }
                else
                {
                    jumpTarget = targetPlatform.GetRightPoint();
                }

                moveDirection = jumpTarget.x > transform.position.x ? 1 : -1;
                break;
            case AIActionType.Latch:
                moveDirection = currentLatchPoint.transform.position.x < transform.position.x ? -1 : 1;
                platformer.SetDirectionalInput(new Vector2(moveDirection, 0));
                platformer.OnJumpInputDown();
                break;
        }
    }

    void Update()
    {
        switch (aiState)
        {
            case AIState.SearchingForAction:
                ScanForAvailableActions();
                break;
            case AIState.ExecutingAction:
                ExecuteAction();
                break;
            case AIState.CooldownToSearch:
                CooldownToSearch();
                break;
        }
    }

    //Scans for the actions
    public void ScanForAvailableActions()
    {
        AIActionType resultAction = AIActionType.None;
        GameObject platformTarget = null;
        GameObject latchTarget = null;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, jumpDetectionRange);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].tag == GameConsts.PLATFORM_TAG)
            {
                if (colliders[i].transform.position.y > transform.position.y)
                {
                    platformTarget = colliders[i].gameObject;
                    targetPlatform = platformTarget.GetComponent<Platform>();
                    continue;
                }
            }
        }

        Collider2D[] latches = Physics2D.OverlapCircleAll(transform.position, latchDetectRange);
        for (int i = 0; i < latches.Length; i++)
        {
            if (latches[i].tag == GameConsts.LATCHPOINT_TAG)
            {
                if (latches[i].transform.position.y > transform.position.y && latches[i].gameObject != currentLatchPoint)
                {
                    currentLatchPoint = latches[i].gameObject;
                    latchTarget = latches[i].gameObject;
                    continue;
                }
            }
        }

        if (platformTarget == null && latchTarget == null && platformer.controller.collisions.below)
        {
            resultAction = AIActionType.Move;
        }
        else if (platformTarget == null && latchTarget != null)
        {
            resultAction = AIActionType.Latch;
        }
        else if (platformTarget != null && latchTarget == null)
        {
            resultAction = AIActionType.Jump;
        }
        else if(platformTarget != null && latchTarget != null)
        {
            resultAction = UnityEngine.Random.Range(0, 100) % 2 == 0 ? AIActionType.Jump : AIActionType.Latch;
        }
        else
        {
            resultAction = AIActionType.Idle;
        }

        if (resultAction != currentAction)
        {
            SetAction(resultAction);
            SetState(AIState.ExecutingAction);
        }

    }

    private void ExecuteAction()
    {
        switch (currentAction)
        {
            case AIActionType.Idle:
                if (platformer.controller.collisions.below)
                {
                    ActionCompleted();
                }
                
                if(lastAction == AIActionType.Latch)
                {
                    if((DateTime.Now - actionSetTime).TotalMilliseconds >= latchWaitTimeAfterIdle * 1000f)
                    {
                        ActionCompleted();
                    }
                }
                break;
            case AIActionType.Move:
                platformer.SetDirectionalInput(new Vector2(moveDirection, 0));

                if(Vector3.Distance(transform.position, moveTarget) <= 1f || platformer.controller.collisions.left || platformer.controller.collisions.right)
                {
                    ActionCompleted();
                }

                if(moveTarget.y > transform.position.y && platformer.controller.collisions.below)
                {
                    ActionCompleted();
                }

                break;
            case AIActionType.Jump:
                if (!jumped)
                {
                    moveDirection = jumpTarget.x > transform.position.x ? 1 : -1;
                    platformer.SetDirectionalInput(new Vector2(moveDirection, 0));
                }

                if(Vector3.Distance(transform.position, jumpTarget) <= 1f)
                {
                    moveDirection = targetPlatform.transform.position.x < transform.position.x ? -1 : 1;
                    platformer.SetDirectionalInput(new Vector2(moveDirection, 0));
                    platformer.OnJumpInputDown();

                    jumped = true;
                    jumpStartTime = DateTime.Now;
                }

                if(jumped && (DateTime.Now - jumpStartTime).TotalMilliseconds >= 500f)
                {
                    if (platformer.controller.collisions.below)
                    {
                        ActionCompleted();
                        jumped = false;
                    }
                }
                break;
            case AIActionType.Latch:

                if(Vector3.Distance(transform.position, currentLatchPoint.transform.position) <= platformer.GetLatchDetectionRadius() && !latched)
                {
                    platformer.SetDirectionalInput(new Vector2(0, 0));
                    platformer.OnLatchInputDown();
                    latched = true;
                    latchStartTime = DateTime.Now;
                }

                if(Vector3.Distance(transform.position, currentLatchPoint.transform.position) >= latchRejectRange && !latched)
                {
                    platformer.OnLatchInputUp();
                    currentLatchPoint = null;
                    ActionCompleted();
                }

                if (latched)
                {
                    if(transform.position.y > currentLatchPoint.transform.position.y && (DateTime.Now - latchStartTime).TotalMilliseconds >= latchWaitTime * 1000f)
                    {
                        latched = false;
                        platformer.OnLatchInputUp();
                        currentLatchPoint = null;
                        ActionCompleted();
                    }
                }
                break;
        }
    }

    private void CooldownToSearch()
    {
        switch (currentAction)
        {
            case AIActionType.Move:
                platformer.SetDirectionalInput(Vector2.zero);
                break;
            case AIActionType.Jump:
                platformer.SetDirectionalInput(Vector2.zero);
                break;
            case AIActionType.Latch:
                break;
        }

        if ((DateTime.Now - cooldownStartTime).TotalMilliseconds >= searchCooldown * 1000)
        {
            SetState(AIState.SearchingForAction);
        }
    }

    public void ActionCompleted()
    {
        //Start the cooldown for waiting
        SetState(AIState.CooldownToSearch);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, latchDetectRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, jumpDetectionRange);

        Gizmos.DrawSphere(moveTarget, 0.5f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(jumpTarget, 0.3f);

        if (currentLatchPoint)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(currentLatchPoint.transform.position, 0.5f);
        }
    }


}