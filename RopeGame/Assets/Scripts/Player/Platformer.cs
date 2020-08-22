using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Core;

[RequireComponent(typeof(Controller2D))]
public class Platformer : MonoBehaviour
{
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
        [SerializeField] private float bandTargetLength;
        [SerializeField] private float bandForce;

        private Vector2 bandCastedPoint;
        #endregion

    #region Referenes
    public Controller2D controller { get; private set; }
    #endregion

    #region Animation variables
        private PlayerLocation playerLocation;
        private PlayerState currentState;
        private float animScale;

    #endregion

    private bool initialized = false;

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

    private void OnEnable()
    {
        GameEventManager.Instance.AddListener<GameStateChangedEvent>(OnGameStateChanged);   
    }

    private void OnDisable()
    {
        GameEventManager.Instance.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
    }

    void Update()
    {
        if (initialized)
        {
            UpdateLatchPointsData();
            CalculateVelocityInState();

            controller.Move(velocity * Time.deltaTime, directionalInput);

            RenderLatchIndicator();

            //If collision with top or bottom then stop the player
            if (controller.collisions.above || controller.collisions.below)
            {
                velocity.y = 0;
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == GameConsts.ENDPOINT_TAG)
        {
            GameEventManager.Instance.TriggerSyncEvent(new ObjectReachedEndEvent(controller.objectType, gameObject));
        }
    }

    #endregion

    #region States related

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        switch (e.gameState)
        {
            case GameState.CountDown:
                initialized = false;
                break;
            case GameState.Match:
                //if(controller.objectType == ObjectType.Player)
                    initialized = true;
                break;
            case GameState.EndSequence:
                initialized = false;
                break;
        }
    }

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
                if (controller.objectType == ObjectType.Player)
                    Camera.main.GetComponent<CameraFollow>().OnLatchEnded();
                break;
            case PlayerState.Jump:
                break;
            case PlayerState.LatchUnderProcess:
                currentLatchingPoint = GetClosestLatchPoint();
                break;
            case PlayerState.Latched:
                if(controller.objectType == ObjectType.Player)
                    Camera.main.GetComponent<CameraFollow>().OnLatched(currentLatchingPoint);
                break;
            case PlayerState.ReleasedFromLatch:
                if (controller.objectType == ObjectType.Player)
                    Camera.main.GetComponent<CameraFollow>().OnLatchEnded();
                break;

        }
    }

    void CalculateVelocityInState()
    {
        float targetVelocityX = 0;

        switch (currentState)
        {
            case PlayerState.None:
                break;

            case PlayerState.IdleMove:
            case PlayerState.Jump:
                //Directional motion and directional controlled motion in jump
                targetVelocityX = directionalInput.x * moveSpeed;
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
                velocity.y += gravity * Time.deltaTime;
                break;

            case PlayerState.LatchUnderProcess:
                //Normal motion till reached latch distance then set to latch state
                targetVelocityX = directionalInput.x * moveSpeed;
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
                velocity.y += gravity * Time.deltaTime;

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

                float angle = Vector2.SignedAngle(currentLatchingPoint.right, ((Vector2)currentLatchingPoint.position - (Vector2)transform.position).normalized);

                if (transform.position.y > currentLatchingPoint.position.y)
                {
                    angle += 180;
                }
                else
                {
                    angle -= 180;
                }

                angle += latchRotationSpeed * rotationDirection;

                velocity.x = ((latchDetectRadius * Mathf.Cos(angle * Mathf.Deg2Rad)) + currentLatchingPoint.position.x - transform.position.x) / Time.deltaTime;
                velocity.y = ((latchDetectRadius * Mathf.Sin(angle * Mathf.Deg2Rad)) + currentLatchingPoint.position.y - transform.position.y) / Time.deltaTime;

                //transform.GetChild(0).right = velocity.normalized;

                break;

            case PlayerState.ReleasedFromLatch:
                //When released from latch, Normal motion with initial latch velocity and no directional input while in air
                targetVelocityX = latchEndVelocity.x;
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
                velocity.y += gravity * Time.deltaTime;
                break;
        }

        Debug.DrawRay(transform.position, velocity.normalized, Color.green);
    }

    #endregion

    #region Getter and Setter
    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public float GetLatchDetectionRadius()
    {
        return latchDetectRadius;
    }
    #endregion

    #region Movement related code
    public void OnJumpInputDown()
    {
        if (controller.collisions.below || controller.collisions.secondaryContact)
        {
            velocity.y = maxJumpVelocity;
            SetPlayerState(PlayerState.Jump);
        }
        else if (controller.collisions.jumpBuffer)
        {
            //Store the jump
            //simulate a jump after player lands on the platform
            jumpStored = true;
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
        if(currentLatchIndicatorParentsMap.Count > 0)
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
        else if(controller.collisions.below)
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
            if(item.Key.transform == currentLatchingPoint)
            {
                if(currentState == PlayerState.LatchUnderProcess)
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

            if(hitInfo.transform != null)
            {
                bandCastedPoint = hitInfo.point;
            }
        }

        public void OnBandInputUp()
        {
            
        }

        #endregion
}