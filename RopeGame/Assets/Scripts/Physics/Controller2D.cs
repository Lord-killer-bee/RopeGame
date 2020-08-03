using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RaycastController
{
    public CollisionInfo collisions;

    [SerializeField] private ObjectType objectType;

    public override void Start()
    {
        base.Start();

        collisions.faceDir = 1;
    }

    /// <summary>
    /// Moves the object, updates raycasts and checks for collisions
    /// </summary>
    /// <param name="moveAmount"></param>
    /// <param name="input"></param>
    /// <param name="standingOnPlatform"></param>
    public void Move(Vector2 moveAmount, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();

        collisions.Reset();

        if (moveAmount.x != 0)
        {
            collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
        }

        //if (moveAmount.x != 0)
        {
            HorizontalCollisions(ref moveAmount);
        }

        //if (moveAmount.y != 0) //Remove to allow collision calculation when banded on wall
        {
            VerticalCollisions(ref moveAmount);
        }

        transform.Translate(moveAmount);

        Physics2D.SyncTransforms();

        if (standingOnPlatform)
        {
            collisions.below = true;
        }
    }

    void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = collisions.faceDir;
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

        if (Mathf.Abs(moveAmount.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                moveAmount.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;

                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }
        }
    }

    void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;
        int jumpBufferCount = 0;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                moveAmount.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }

            if (objectType == ObjectType.Player && moveAmount.y < 0)
            {
                RaycastHit2D longhit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength * 5, collisionMask);
                Debug.DrawRay(rayOrigin, Vector2.down * rayLength * 5, Color.blue);

                if (longhit)
                {
                    jumpBufferCount++;
                }
            }
        }

        collisions.jumpBuffer = jumpBufferCount > 0;

        if(objectType == ObjectType.Player)
        {
            float directionX = collisions.faceDir;

            Vector2 rayOrigin = (directionX == 1) ? raycastOrigins.secondaryBottomLeft : raycastOrigins.secondaryBottomRight;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.down, Color.red);

            // Check if player is over the ledge to avoid jumping along a wall
            if (hit && hit.collider.transform.position.y < transform.position.y)
            {
                collisions.secondaryContact = true;
            }
        }
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool secondaryContact;//this is for a buffer to jump from the edge
        public bool jumpBuffer;//this is for a buffer to store next jump if jumped when this is true

        public int faceDir;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            secondaryContact = false;
        }
    }

}