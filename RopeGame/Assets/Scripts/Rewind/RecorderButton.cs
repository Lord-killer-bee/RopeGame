using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class RecorderButton : MonoBehaviour
{
    [SerializeField] private RecorderButtonType buttonType;
    [SerializeField] private int groupID;

    private bool isActive;
    private bool isTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == GameConsts.PLAYER_TAG)
        {
            isTriggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == GameConsts.PLAYER_TAG)
        {
            isTriggered = false;
        }
    }

    private void Update()
    {
        if (isTriggered)
        {
            ActivateButtonEffect();
            gameObject.SetActive(false);
        }
    }

    private void ActivateButtonEffect()
    {
        GameEventManager.Instance.TriggerAsyncEvent(new ActivateButtonEffect(buttonType, groupID));
    }
}
