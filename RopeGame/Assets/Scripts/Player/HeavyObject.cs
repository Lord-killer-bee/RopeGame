using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class HeavyObject : MonoBehaviour
{
    private bool levelInitiated;
    public Controller2D controller { get; private set; }

    Vector3 velocity;
    private float gravity;

    void Awake()
    {
        controller = GetComponent<Controller2D>();
        gravity = -9.81f;
    }

    private void OnEnable()
    {
        GameEventManager.Instance.AddListener<LevelInitiatedEvent>(OnLevelInitiated);
    }

    private void OnDisable()
    {
        GameEventManager.Instance.RemoveListener<LevelInitiatedEvent>(OnLevelInitiated);
    }

    private void OnLevelInitiated(LevelInitiatedEvent e)
    {
        levelInitiated = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == GameConsts.SPIKE_TAG)
        {
            GameEventManager.Instance.TriggerSyncEvent(new KeyHitSpikeEvent());
            GameEventManager.Instance.TriggerAsyncEvent(new ShakeCameraEvent());
        }
    }

    void Update()
    {
        if (levelInitiated)
        {
            CalculateVelocity();

            controller.Move(velocity * Time.deltaTime);

            //If collision with top or bottom then stop the player
            if (controller.collisions.above || controller.collisions.below)
            {
                velocity.y = 0;
            }
        }
    }

    private void CalculateVelocity()
    {
        velocity.y += gravity * Time.deltaTime;
    }
}
