using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyObject : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == GameConsts.SPIKE_TAG)
        {
            GameEventManager.Instance.TriggerSyncEvent(new KeyHitSpikeEvent());
            GameEventManager.Instance.TriggerAsyncEvent(new ShakeCameraEvent());
            //gameObject.SetActive(false);
            animator.Play("Destroy");
        }
    }
}
