using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveTest : MonoBehaviour
{
    public WheelCollider[] WCs;
    public GameObject[] Wheels;
    public float maxSteerAngle = 30;
    public float torque = 200;
    // Start is called before the first frame update
    void Start()
    {

    }
    void Go(float accel, float steer)
    {
        accel = Mathf.Clamp(accel, -1, 1);
        steer = Mathf.Clamp(steer, -1, 1) * maxSteerAngle;
        float thrustTorque = accel * torque;
        for (int i = 0; i < 4; i++)
        {
            WCs[i].motorTorque = thrustTorque;
            if(i<2)
            WCs[i].steerAngle = steer;

            Quaternion quat;
            Vector3 postion;
            WCs[i].GetWorldPose(out postion, out quat);
            Wheels[i].transform.position = postion;
            Wheels[i].transform.rotation = quat;
        }


    }
    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        float s = Input.GetAxis("Horizontal");
        Go(a, s);
    }
}
