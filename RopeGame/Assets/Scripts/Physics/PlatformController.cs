using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{
    public LayerMask passengerMask;

    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;

    public float speed;
    public float speedMultiplier;

    [HideInInspector] public int startIndex;
    [HideInInspector] public int targetIndex;
    [HideInInspector] public bool shouldMove;
    [HideInInspector] public bool isSpedUp;

    int fromWaypointIndex;
    int toWaypointIndex;
    bool switchDirection;

    bool initialized;

    List<PassengerMovement> passengerMovement;
    Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

    public void Initialize()
    {
        base.Start();
        initialized = true;

        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }

        fromWaypointIndex = 0;
        toWaypointIndex = 1;
    }

    public void UpdateMotion(Vector3 velocity)
    {
        CalculatePassengerMovement(velocity);
        MovePassengers(true);
        transform.Translate(velocity);
        Physics2D.SyncTransforms();
        MovePassengers(false);
    }

    public void SetPositionTo(int wayPointIndex)
    {
        transform.position = globalWaypoints[wayPointIndex];
    }

    public void SwitchDirection()
    {
        if(shouldMove)
            switchDirection = true;
    }

    public Vector3 CalculatePlatformMovement()
    {
        if (!shouldMove)
        {
            return Vector3.zero;
        }

        if (switchDirection)
        {
            switchDirection = false;

            if(targetIndex > 0)
            {
                if (fromWaypointIndex == globalWaypoints.Length - 1)
                {
                    toWaypointIndex = globalWaypoints.Length - 1;
                    fromWaypointIndex = toWaypointIndex - 1;
                }
                else
                {
                    toWaypointIndex = fromWaypointIndex + 1;
                }
            }
            else
            {
                if (fromWaypointIndex == 0)
                {
                    fromWaypointIndex = 1;
                    toWaypointIndex = 0;
                }
                else
                {
                    toWaypointIndex = fromWaypointIndex - 1;
                }
            }
        }
        else if (Vector3.Distance(transform.position, globalWaypoints[toWaypointIndex]) == 0)
        {
            if (targetIndex > 0)
            {
                if (toWaypointIndex < globalWaypoints.Length - 1)
                {
                    fromWaypointIndex++;
                    toWaypointIndex++;
                }
            }
            else
            {
                if (toWaypointIndex > 0)
                {
                    fromWaypointIndex--;
                    toWaypointIndex--;
                }
            }
        }

        Vector3 newPos = Vector3.MoveTowards(transform.position, globalWaypoints[toWaypointIndex], speed * (isSpedUp ? speedMultiplier : 1) * Time.deltaTime);
        return newPos - transform.position;
    }

    void MovePassengers(bool beforeMovePlatform)
    {
        foreach (PassengerMovement passenger in passengerMovement)
        {
            if (!passengerDictionary.ContainsKey(passenger.transform))
            {
                passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<Controller2D>());
            }

            if (passenger.moveBeforePlatform == beforeMovePlatform)
            {
                passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
            }
        }
    }

    void CalculatePassengerMovement(Vector3 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        // Vertically moving platform
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

                if (hit && hit.distance != 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = (directionY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                    }
                }

                
            }
        }

        // Horizontally moving platform
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

                if (hit && hit.distance != 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                        float pushY = -skinWidth;

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                    }
                }
            }
        }

        // Passenger on top of a horizontally or downward moving platform
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = 0;

            if(velocity.y == 0)
            {
                rayLength = 2 * skinWidth;
            }
            else
            {
                rayLength = Mathf.Abs(velocity.y) + skinWidth;

            }

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                if (hit && hit.distance != 0)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                    }
                }

                Debug.DrawRay(rayOrigin, Vector2.up * rayLength, Color.green);
            }
        }
    }

    struct PassengerMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform)
        {
            transform = _transform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }
    }

    void OnDrawGizmos()
    {
        if (localWaypoints != null && initialized)
        {
            Gizmos.color = Color.red;
            float size = .3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
        }
    }

}
