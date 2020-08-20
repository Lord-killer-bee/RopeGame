using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformController))]
public class RewindableTranslatingPlatform : RewindableEntity
{
    private PlatformController platformController;

    #region Base methods

    public override void OnLevelInitiated()
    {
        base.OnLevelInitiated();

        platformController = GetComponent<PlatformController>();
        platformController.Initialize();

        //Set starting position
        transform.position = platformController.localWaypoints[0] + transform.position;
        PlayEntity();
    }

    public override void PlayEntity()
    {
        base.PlayEntity();

        //Set current move target to end point
        platformController.SwitchDirection();
        platformController.targetIndex = platformController.localWaypoints.Length - 1;
        platformController.shouldMove = true;
        platformController.isSpedUp = false;
    }

    public override void PauseEntity()
    {
        base.PauseEntity();

        platformController.shouldMove = false;
    }

    public override void RewindEntity()
    {
        base.RewindEntity();

        //Set current move target to start point
        platformController.targetIndex = 0;
        platformController.shouldMove = true;
        platformController.SwitchDirection();

        Debug.Log("Rewinded");
    }

    public override void StopEntity()
    {
        base.StopEntity();

        platformController.shouldMove = false;
        platformController.isSpedUp = false;
        platformController.SetPositionTo(0);
    }

    public override void FastForwardEntity()
    {
        base.FastForwardEntity();

        platformController.shouldMove = true;
        platformController.isSpedUp = true;
    }

    #endregion

    private void Update()
    {
        if (levelInitiated)
        {
            Vector3 velocity = platformController.CalculatePlatformMovement();

            platformController.UpdateRaycastOrigins();
            platformController.UpdateMotion(velocity);
        }
    }

}
