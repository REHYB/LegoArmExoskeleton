using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Udp;

public class ExampleController : MonoBehaviour
{
    public UdpHost udpHost;
    public Transform cube;
    private float motor_angle;

    // Start is called before the first frame update
    void Start()
    {
        ExampleEvents.current.onMKeyHit += OnMKeyhit;
        UdpHost.OnReceiveMsg += OnMotorValue;
    }

    // Update is called once per frame
    void Update()
    {
        // Checking when the key "m" is pressed
        if (Input.GetKeyDown("m")) {
            // If it's pressed, trigger the event which we specified in our ExampleEvents.cs
            ExampleEvents.current.MKeyHit();
        }

        cube.position = new Vector3(motor_angle / 100f, 0f, 0f);
    }

    private void OnMKeyhit()
    {
        Debug.Log("pressed");
        udpHost.SendMsg("move");

    }

    private void OnMotorValue(string value)
    {
        motor_angle = float.Parse(value);
        //Debug.Log("This is the received value: " + value);
    }
}
