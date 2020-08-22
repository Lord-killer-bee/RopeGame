using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private Transform leftJumpPoint;
    [SerializeField] private Transform rightJumpPoint;

    public Vector3 GetLeftPoint()
    {
        return leftJumpPoint.transform.position;
    }

    public Vector3 GetRightPoint()
    {
        return rightJumpPoint.transform.position;
    }
}
