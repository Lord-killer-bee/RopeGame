using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindingEnemy : MonoBehaviour
{
    [SerializeField] private WindingManager windingManager;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == GameConsts.WINDINGTARGET_TAG)
        {
            windingManager.EnemyReachedTarget();
        }
    }
}
