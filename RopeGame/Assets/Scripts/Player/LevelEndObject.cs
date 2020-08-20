using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndObject : MonoBehaviour
{
    [SerializeField] private bool isLocked;

    public bool IsLocked()
    {
        return isLocked;
    }
}
