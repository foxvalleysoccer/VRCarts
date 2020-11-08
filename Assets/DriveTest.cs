using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveTest : MonoBehaviour
{
    public WheelCollider WC;
    public GameObject Wheels;

    public float torque = 200;
    // Start is called before the first frame update
    void Start()
    {
        WC = this.GetComponent<WheelCollider>();
    }
    void Go(float accel )
    {
        accel = Mathf.Clamp(accel, -1, 1);
        float thrustTorque = accel * torque;
        WC.motorTorque = thrustTorque;
    }
    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        Go(a);
    }
}
