using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class DriveTest : MonoBehaviour
{
    List<InputDevice> inputDevices;
    public XRNode controllerNode;
    public WheelCollider[] WCs;
    public GameObject[] Wheels;
    public float maxSteerAngle = 30;
    public float torque = 200;
    public float maxBrakeTorque = 500;
    public InputDeviceCharacteristics deviceCharacteristics;
    public List<XRController> controllers;

    public AudioSource skidSound;
    public Transform skidTrailPrefab;
    public Transform[] skidTrails = new Transform[4];
    public ParticleSystem smokePrefab;
    ParticleSystem[] skidSmoke = new ParticleSystem[4];
    public GameObject brakeLight;
    void Start()
    {
        for (int i = 0;i<4;i++)
        {
            skidSmoke[i] = Instantiate(smokePrefab);
            skidSmoke[i].Stop();
        }
        brakeLight.SetActive(false);
    }
    public void StartSkidTrail(int i)
    {
        if (skidTrails[i] == null)
        {
            skidTrails[i] = Instantiate(skidTrailPrefab);
        }
        skidTrails[i].parent = WCs[i].transform;
        //helps keep skid marks on ground
        skidTrails[i].localRotation = Quaternion.Euler(90, 0, 0);
        skidTrails[i].localPosition = -Vector3.up * WCs[i].radius;
    }
    public void EndSkidTrail(int i)
    {
        if (skidTrails[i] == null)
        {
            return;
        }
        Transform holder = skidTrails[i];
        skidTrails[i] = null;
        holder.parent = null;
        //this also helps keep skid marks on ground
        holder.localRotation = Quaternion.Euler(90, 0, 0);
        Destroy(holder.gameObject, 30);
    }
    // Start is called before the first frame update
    void Awake()
    {
        inputDevices = new List<InputDevice>();
    }
    void Go(float accel, float steer, float brake)
    {
        accel = Mathf.Clamp(accel, -5, 5);
        steer = Mathf.Clamp(steer, -1, 1) * maxSteerAngle;
        brake = Mathf.Clamp(brake, 0, 1) * maxBrakeTorque;

        if (brake != 0)
            brakeLight.SetActive(true);
        else
            brakeLight.SetActive(false);


        float thrustTorque = accel * torque;


        for (int i = 0; i < 4; i++)
        {
            WCs[i].motorTorque = thrustTorque;
            if (i < 2)
                WCs[i].steerAngle = steer;
            else
                WCs[i].brakeTorque = brake;

            Quaternion quat;
            Vector3 postion;
            WCs[i].GetWorldPose(out postion, out quat);
            Wheels[i].transform.position = postion;
            Wheels[i].transform.rotation = quat;
        }


    }
    public void CheckForSkid()
    {
        int numSkidding = 0;
        for (int i = 0; i<4;i++)
        {
            WheelHit wheelHit;
            WCs[i].GetGroundHit(out wheelHit);
            if (Mathf.Abs(wheelHit.forwardSlip) > 0.4f || Mathf.Abs(wheelHit.sidewaysSlip)>0.4f)
            {
                numSkidding++;
                if (!skidSound.isPlaying)
                {
                    skidSound.Play();
                    StartSkidTrail(i);
                    skidSmoke[i].transform.position = WCs[i].transform.position - WCs[i].transform.up * WCs[i].radius;
                    skidSmoke[i].Emit(1);
                }
            }
            else
            {
                EndSkidTrail(i);
            }
        }
        if (numSkidding == 0 && skidSound.isPlaying)
        {
            skidSound.Stop();
        }
    }
    // Update is called once per frame
    void Update()
    {
        deviceCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeldInHand |
            InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Right;
        InputDevices.GetDevicesAtXRNode(controllerNode, inputDevices);
        float a = 0.0f;
        float b = 0.0f;
        float s = 0.0f;

        foreach (InputDevice inputDevice in inputDevices)
        {
            bool primaryinputValue;
            bool secondaryinputValue;
            float triggerInputValue;

            // Debug.Log("Device: " + inputDevice.name);
            if (inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out primaryinputValue) && primaryinputValue)
            {
                Debug.Log("primary button" + primaryinputValue.ToString());
                a = 20.0f;
            }
            if (inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryinputValue) && secondaryinputValue)
            {
                Debug.Log("secondary button" + primaryinputValue.ToString());
                b = 30.0f;
            }
            if (inputDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerInputValue))
            {
                Debug.Log("Trigger pressed :" + triggerInputValue.ToString());
                //if (triggerInputValue > 0.5f)
                //{
                //    a = -10.0f;
                //}

            }

        }
        foreach (XRController xRController in controllers)
        {
            if (xRController.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 positionVector))
            {
                Debug.Log("positionVector.x = : " + positionVector.x);
                //Move(positionVector);
                s = positionVector.x;
            }
            Debug.Log(xRController.inputDevice.name);
            //if (xRController.inputDevice.)
            //{

            //}
            if (xRController.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool aPressed))
            {
                if (aPressed == true)
                {
                    Debug.Log("a pressed");
                    a = -10.0f;
                }

            }

        }

        Go(a, s, b);
        CheckForSkid();
    }
}
