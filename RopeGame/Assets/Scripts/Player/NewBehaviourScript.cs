using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveAmount;
        float angle = Vector2.SignedAngle(Vector2.right, (new Vector2(0, 0) - (Vector2)transform.position).normalized);

        if (transform.position.y > 0)
        {
            angle += 180;
        }
        else
        {
            angle -= 180;
        }

        angle += 75 * Time.deltaTime;

        moveAmount.x = (3 * Mathf.Cos(angle * Mathf.Deg2Rad)) - transform.position.x;
        moveAmount.y = (3 * Mathf.Sin(angle * Mathf.Deg2Rad)) - transform.position.y;

        transform.Translate(moveAmount);
    }
}
