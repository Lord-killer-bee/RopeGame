using UnityEngine;
using System.Collections;

namespace ArrowGame
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class RaycastController : MonoBehaviour
    {

        public LayerMask collisionMask;

        public const float skinWidth = .015f;
        const float dstBetweenRays = .25f;
        const float dstSecondaryBottom = .3f;
        [HideInInspector]
        public int horizontalRayCount;
        [HideInInspector]
        public int verticalRayCount;

        [HideInInspector]
        public float horizontalRaySpacing;
        [HideInInspector]
        public float verticalRaySpacing;

        [HideInInspector]
        public BoxCollider2D collider;
        public RaycastOrigins raycastOrigins;

        public virtual void Awake()
        {
            collider = GetComponent<BoxCollider2D>();
        }

        /// <summary>
        /// Calculate ray spacing once object is created
        /// </summary>
        public virtual void Start()
        {
            CalculateRaySpacing();
        }

        /// <summary>
        /// Updates the raycast points every frame as the player can move
        /// </summary>
        public void UpdateRaycastOrigins()
        {
            Bounds bounds = collider.bounds;
            bounds.Expand(skinWidth * -2);

            raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

            raycastOrigins.secondaryBottomLeft = new Vector2(bounds.min.x - dstSecondaryBottom, bounds.min.y);
            raycastOrigins.secondaryBottomRight = new Vector2(bounds.max.x + dstSecondaryBottom, bounds.min.y);
        }

        /// <summary>
        /// Calculates ray spacing based on object's bounds dimensions
        /// </summary>
        public void CalculateRaySpacing()
        {
            Bounds bounds = collider.bounds;
            bounds.Expand(skinWidth * -2);

            float boundsWidth = bounds.size.x;
            float boundsHeight = bounds.size.y;

            horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
            verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);

            horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
            verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
        }

        /// <summary>
        /// Struct to hold raycast origin dadas
        /// </summary>
        public struct RaycastOrigins
        {
            public Vector2 topLeft, topRight;
            public Vector2 bottomLeft, bottomRight;
            public Vector2 secondaryBottomLeft, secondaryBottomRight;//this is for a buffer to jump from the edge
        }
    }
}