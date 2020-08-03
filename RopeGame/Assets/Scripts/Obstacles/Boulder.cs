using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Boulder : MonoBehaviour
{
    [SerializeField] private BoulderType boulderType;
    [SerializeField] private float mass;
    [SerializeField] private float hitsToBreak;

    private Vector3 velocity;
    bool forceApplied = false;
    float currentHitsToBreak;

    #region Referenes
    public Controller2D controller { get; private set; }
    #endregion

    private void Awake()
    {
        controller = GetComponent<Controller2D>();
        currentHitsToBreak = hitsToBreak;
    }

    private void Update()
    {
        CalculateVelocity();
        controller.Move(velocity * Time.deltaTime);
    }

    private void CalculateVelocity()
    {
        if (forceApplied)
        {
            velocity.x -= mass * Mathf.Sign(velocity.x);

            if (Mathf.Abs(velocity.x) <= 0)
            {
                forceApplied = false;
                velocity.x = 0;
            }
        }
    }

    public void ApplyForce(float force, Vector2 direction)
    {
        if (boulderType == BoulderType.Movable)
        {
            velocity.x = force * direction.x;
            forceApplied = true;
        }
        else if (boulderType == BoulderType.Destructible)
        {
            DamageBoulder();
        }
    }

    private void DamageBoulder()
    {
        currentHitsToBreak--;

        GetComponent<SpriteRenderer>().color = Color.white * (currentHitsToBreak / hitsToBreak);

        if (currentHitsToBreak == 0)
            Destroy(gameObject);
    }
}
