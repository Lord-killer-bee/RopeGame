using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "player")
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
