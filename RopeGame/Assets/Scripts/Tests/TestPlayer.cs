using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Controller2D))]
public class TestPlayer : MonoBehaviour
{
    public RangeTestManager testManager;

    #region Constants
    private const float maxIndicatorLength = 14.732f;
    #endregion

    #region Movement related variables
    [Header("Movement related")]
    [SerializeField] private float moveSpeed = 6;
    [SerializeField] private float maxJumpHeight = 4;
    [SerializeField] private float minJumpHeight = 1;
    [SerializeField] private float timeToJumpApex = .4f;

    public Vector2 directionalInput { get; private set; }
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .01f;
    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;
    private bool jumpStored = false;

    #endregion

    #region Latching related variables
    [Header("Latching related")]
    [SerializeField] private float latchDetectRadius = 4f;
    [SerializeField] private float latchRotationSpeed = 4f;
    [SerializeField] private GameObject latchIndicatorParentPref;

    private int rotationDirection = 1;
    private Vector2 latchEndVelocity = Vector2.zero;
    private Dictionary<Collider2D, LatchLine> currentLatchIndicatorParentsMap = new Dictionary<Collider2D, LatchLine>();
    private Transform currentLatchingPoint;//Only exists during latching under process and latching phase

    #endregion

    #region Band related variables
    [Header("Banding related")]
    [SerializeField] private GameObject bandRopePref;
    [SerializeField] private float bandTargetLength;
    [SerializeField] private float bandForce;
    [SerializeField] private float frictionWhenBanded;
    [SerializeField] private Vector2 jumpForceFromWall;
    [SerializeField] private float wallJumpAirFriction;
    [SerializeField] private float forceAppliedOnWall;

    private Vector2 bandTargetPoint;
    private Transform bandHitTransform;
    private bool bandHeld = false;
    private GameObject bandRopeObj;
    private int wallJumpDirection;
    #endregion

    #region Referenes
    public Controller2D controller { get; private set; }
    #endregion

    #region Animation variables
    private PlayerLocation playerLocation;
    private PlayerState currentState;
    private float animScale;

    #endregion

    #region Unity base methods

    void Awake()
    {
        controller = GetComponent<Controller2D>();

        //Set gravity and velocities based on kinematic equations
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        SetPlayerState(PlayerState.IdleMove);
    }

    void Update()
    {
        UpdateLatchPointsData();
        CalculateVelocityInState();

        controller.Move(velocity * Time.deltaTime);

        RenderLatchIndicator();

        //If collision with top or bottom then stop the player
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;

            testManager.NextTest();
        }

        if (controller.collisions.below)
        {
            ChangePlayerLocation(PlayerLocation.Grounded);

            //If jump is stored then simulate a jump even with out any further input
            if (jumpStored)
            {
                OnJumpInputDown();
                jumpStored = false;
            }
        }
        else
            ChangePlayerLocation(PlayerLocation.InAir);
    }


    #endregion

    #region States related

    void ChangePlayerLocation(PlayerLocation location)
    {
        if (playerLocation == location)
            return;

        switch (location)
        {
            case PlayerLocation.Grounded:
                //When grounded remove the velocity due to latch eject and update the status
                latchEndVelocity = Vector2.zero;
                SetPlayerState(PlayerState.IdleMove);
                break;
            case PlayerLocation.InAir:
                break;
        }

        playerLocation = location;
    }

    void SetPlayerState(PlayerState state)
    {
        if (currentState == state)
            return;

        currentState = state;

        switch (state)
        {
            case PlayerState.None:
                break;
            case PlayerState.IdleMove:
                break;
            case PlayerState.Jump:
                break;
            case PlayerState.LatchUnderProcess:
                currentLatchingPoint = GetClosestLatchPoint();
                break;
            case PlayerState.Latched:
                break;
            case PlayerState.ReleasedFromLatch:
                break;

        }
    }

    float angle = 180;

    void CalculateVelocityInState()
    {
        float targetVelocityX = 0;

        switch (currentState)
        {
            case PlayerState.None:
                break;

            case PlayerState.IdleMove:
            case PlayerState.Jump:
                velocity = Vector2.zero;
                //Directional motion and directional controlled motion in jump
                //targetVelocityX = directionalInput.x * moveSpeed;
                //velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
                //velocity.y += gravity * Time.deltaTime;
                break;

            case PlayerState.LatchUnderProcess:
                //Normal motion till reached latch distance then set to latch state
                //targetVelocityX = directionalInput.x * moveSpeed;
                //velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
                //velocity.y += gravity * Time.deltaTime;

                if (currentLatchingPoint)
                {
                    if (Vector3.Distance(currentLatchingPoint.position, transform.position) >= latchDetectRadius)
                    {
                        SetPlayerState(PlayerState.Latched);
                        rotationDirection = -(int)Mathf.Sign(Vector3.Cross(currentLatchingPoint.position - transform.position, velocity).z);
                    }
                }
                break;

            case PlayerState.Latched:
                //Rotate in a circle based on the velocity angle.
                //Speed is constant right now

                angle = (int)Vector2.SignedAngle(currentLatchingPoint.right, ((Vector2)currentLatchingPoint.position - (Vector2)transform.position).normalized);

                if (transform.position.y > currentLatchingPoint.position.y)
                {
                    angle += 180;
                }
                else
                {
                    angle -= 180;
                }

                angle += testManager.angleStep * rotationDirection;

                velocity.x = ((latchDetectRadius * Mathf.Cos(angle * Mathf.Deg2Rad)) + currentLatchingPoint.position.x - transform.position.x) / Time.deltaTime;
                velocity.y = ((latchDetectRadius * Mathf.Sin(angle * Mathf.Deg2Rad)) + currentLatchingPoint.position.y - transform.position.y) / Time.deltaTime;
                break;

            case PlayerState.ReleasedFromLatch:
                //When released from latch, Normal motion with initial latch velocity and no directional input while in air
                targetVelocityX = latchEndVelocity.x;
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
                velocity.y += gravity * Time.deltaTime;
                break;

            case PlayerState.MoveByBand:

                bandRopeObj.GetComponent<LineRenderer>().SetPosition(0, (Vector2)transform.position);
                bandRopeObj.GetComponent<LineRenderer>().SetPosition(1, bandTargetPoint);

                if (velocity.x > 0)
                    velocity.x -= frictionWhenBanded;

                if (controller.collisions.right || controller.collisions.left)
                {
                    if (bandHeld)
                    {
                        velocity.x = 0;
                        SetPlayerState(PlayerState.StuckOnWall);
                    }
                    else
                    {
                        Destroy(bandRopeObj);
                        SetPlayerState(PlayerState.IdleMove);
                    }

                    Boulder boulder = bandHitTransform.GetComponent<Boulder>();

                    if (boulder)
                    {
                        boulder.ApplyForce(forceAppliedOnWall, transform.right);
                    }
                }
                break;

            case PlayerState.StuckOnWall:
                velocity = Vector2.zero;
                break;

            case PlayerState.JumpFromWall:
                velocity.x -= wallJumpDirection * wallJumpAirFriction;
                velocity.y -= wallJumpAirFriction;

                if (Mathf.Abs(velocity.x) <= 0.1f)
                {
                    SetPlayerState(PlayerState.IdleMove);
                }

                break;
        }

        Debug.DrawRay(transform.position, velocity.normalized, Color.green);
    }

    public void ReleaseLatch()
    {
        OnLatchInputUp();
    }

    #endregion

    #region Getter and Setter
    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }
    #endregion

    #region Movement related code
    public void OnJumpInputDown()
    {
        if (controller.collisions.below || controller.collisions.secondaryContact)
        {
            if (bandRopeObj)
                Destroy(bandRopeObj);

            velocity.y = maxJumpVelocity;
            SetPlayerState(PlayerState.Jump);
        }
        else if (controller.collisions.jumpBuffer)
        {
            //Store the jump
            //simulate a jump after player lands on the platform
            jumpStored = true;
        }
        else if (currentState == PlayerState.StuckOnWall)
        {
            Destroy(bandRopeObj);

            wallJumpDirection = transform.position.x > bandTargetPoint.x ? 1 : -1;
            velocity = new Vector2(wallJumpDirection * jumpForceFromWall.x, jumpForceFromWall.y);
            SetPlayerState(PlayerState.JumpFromWall);
        }
    }

    public void OnJumpInputUp()
    {
        //DOUBT: Didn't get this
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    #endregion

    #region Latch related code

    public void OnLatchInputDown()
    {
        if (currentLatchIndicatorParentsMap.Count > 0)
        {
            SetPlayerState(PlayerState.LatchUnderProcess);
        }
    }

    public void OnLatchInputUp()
    {
        if (!controller.collisions.below && currentState == PlayerState.Latched)
        {
            latchEndVelocity = velocity;
            SetPlayerState(PlayerState.ReleasedFromLatch);
        }
        else if (controller.collisions.below)
        {
            SetPlayerState(PlayerState.IdleMove);
        }

        currentLatchingPoint = null;
    }

    private void UpdateLatchPointsData()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, latchDetectRadius, GameConsts.LATCHPOINTS_LAYER);

        List<Collider2D> keys = new List<Collider2D>(currentLatchIndicatorParentsMap.Keys);
        //Remove out of range ones
        foreach (var item in keys)
        {
            if (!colliders.Contains(item))
            {
                if (!(item.transform == currentLatchingPoint && (currentState == PlayerState.Latched || currentState == PlayerState.LatchUnderProcess)))
                {
                    Destroy(currentLatchIndicatorParentsMap[item].gameObject);
                    currentLatchIndicatorParentsMap.Remove(item);
                }
            }
        }


        for (int i = 0; i < colliders.Length; i++)
        {
            if (!currentLatchIndicatorParentsMap.Keys.Contains(colliders[i]))
            {
                GameObject indicator = Instantiate(latchIndicatorParentPref, latchIndicatorParentPref.transform.parent);
                indicator.SetActive(true);
                currentLatchIndicatorParentsMap.Add(colliders[i], indicator.GetComponent<LatchLine>());
            }
        }
    }

    private Transform GetClosestLatchPoint()
    {
        float minDistance = 1000f;
        Transform result = null;

        foreach (var item in currentLatchIndicatorParentsMap)
        {
            float dist = Vector3.Distance(item.Key.transform.position, transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                result = item.Key.transform;
            }
        }

        return result;
    }

    private void RenderLatchIndicator()
    {
        foreach (var item in currentLatchIndicatorParentsMap)
        {
            //Latched line or under process
            if (item.Key.transform == currentLatchingPoint)
            {
                if (currentState == PlayerState.LatchUnderProcess)
                {
                    item.Value.latchedLine.gameObject.SetActive(false);
                    item.Value.latchIndicatorLine.gameObject.SetActive(true);

                    item.Value.latchIndicatorLine.GetComponent<SpriteRenderer>().color = new Color(1, 0.8f, 0, 1);
                }
                else if (currentState == PlayerState.Latched)
                {
                    item.Value.latchedLine.gameObject.SetActive(true);
                    item.Value.latchIndicatorLine.gameObject.SetActive(false);
                }
                else
                {
                    item.Value.latchedLine.gameObject.SetActive(false);
                    item.Value.latchIndicatorLine.gameObject.SetActive(true);

                    item.Value.latchIndicatorLine.GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
            else
            {
                item.Value.latchedLine.gameObject.SetActive(false);
                item.Value.latchIndicatorLine.gameObject.SetActive(true);

                item.Value.latchIndicatorLine.GetComponent<SpriteRenderer>().color = Color.white;
            }

            item.Value.transform.localEulerAngles = new Vector3(0, 0, Vector3.SignedAngle(transform.right, item.Key.transform.position - transform.position, Vector3.forward));

            item.Value.latchedLine.localPosition = new Vector2(Vector3.Distance(item.Key.transform.position, transform.position) - maxIndicatorLength, 0);
            item.Value.latchIndicatorLine.localPosition = new Vector2(Vector3.Distance(item.Key.transform.position, transform.position) - maxIndicatorLength, 0);
        }
    }

    #endregion

    #region Banding related code

    public void OnBandInputDown()
    {
        //Check for a target surface
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.right, bandTargetLength, GameConsts.PLATFORM_LAYER);

        if (hitInfo.transform != null)
        {
            SetPlayerState(PlayerState.CastingBand);
            velocity.y = 0;

            bandTargetPoint = hitInfo.point;
            bandHitTransform = hitInfo.transform;

            bandRopeObj = Instantiate(bandRopePref);
            bandRopeObj.GetComponent<LineRenderer>().SetPosition(0, (Vector2)transform.position);
            bandRopeObj.GetComponent<LineRenderer>().SetPosition(1, (Vector2)transform.position);

            Invoke("ApplyBandForce", 0.1f);
            bandHeld = true;
        }
    }

    private void ApplyBandForce()
    {
        //Add force in the casted direction
        SetPlayerState(PlayerState.MoveByBand);
        velocity.x = bandForce;
        velocity.y = 0;
    }

    public void OnBandInputUp()
    {
        bandHeld = false;
        if (currentState == PlayerState.StuckOnWall)
        {
            SetPlayerState(PlayerState.IdleMove);
            Destroy(bandRopeObj);
        }
    }

    #endregion
}