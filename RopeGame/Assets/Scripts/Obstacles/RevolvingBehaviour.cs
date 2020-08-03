using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolvingBehaviour : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;

    public void Update()
    {
        transform.Rotate(transform.forward, rotationSpeed * Time.deltaTime);
        transform.GetChild(0).up = Vector3.up;
    }
}
