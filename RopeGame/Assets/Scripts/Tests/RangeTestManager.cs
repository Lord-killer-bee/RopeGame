using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeTestManager : MonoBehaviour
{
    [SerializeField] private TestPlayer player;
    [SerializeField] private Transform trigger;

    private int angle = 0;
    public int angleStep = 2;
    private float waitTimeForNextTest = 0.5f;
    private int rotationDirection = 1;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = player.transform.position;
        angle = 0;
    }

    public void StartTest()
    {
        player.enabled = true;
        Invoke("LatchOn", 1f);
    }

    public void StopTest()
    {
        player.enabled = false;
        player.transform.position = new Vector3(initialPosition.x * rotationDirection, initialPosition.y, 0);
    }

    void LatchOn()
    {
        player.OnLatchInputDown();
    }

    public void NextTest()
    {
        angle -= angleStep;
        trigger.localEulerAngles = new Vector3(0, 0, angle);
        StopTest();
        StartTest();
    }
}
