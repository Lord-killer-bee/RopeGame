using UnityEngine;
using System.Collections;
using Core;
using System;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    Player player;

    void Start()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw(GameConsts.HORIZONTAL_CODE), Input.GetAxisRaw(GameConsts.VERTICAL_CODE));
        player.SetDirectionalInput(directionalInput);

        if (Input.GetButtonDown(GameConsts.JUMP_CODE))
        {
            player.OnJumpInputDown();
        }
        if (Input.GetButtonUp(GameConsts.JUMP_CODE))
        {
            player.OnJumpInputUp();
        }

        if (Input.GetButtonDown(GameConsts.LATCH_CODE))
        {
            player.OnLatchInputDown();
        }
        if (Input.GetButtonUp(GameConsts.LATCH_CODE))
        {
            player.OnLatchInputUp();
        }

        if (Input.GetButtonDown(GameConsts.BAND_CODE))
        {
            player.OnBandInputDown();
        }
        if (Input.GetButtonUp(GameConsts.BAND_CODE))
        {
            player.OnBandInputUp();
        }
    }
}