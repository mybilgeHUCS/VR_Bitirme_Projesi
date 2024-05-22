using UnityEngine;
using System;
using System.Collections.Generic;

public class CarController : MonoBehaviour
{
    public enum ControlMode
    {
        Keyboard,
        VR
    };

    public enum Axel
    {
        Front,
        Rear
    }

    public GameObject steeringWheel;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float maxRPMForVolume = 5000;

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public Axel axel;
    }

    public ControlMode control;

    public float maxAcceleration = 30.0f;
    public float brakeAcceleration = 50.0f;

    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 30.0f;

    public Vector3 _centerOfMass;

    public List<Wheel> wheels;

    public float moveInput;
    public float steerInput;
    public bool isVrBraking;
    [SerializeField] Transform direksiyon;

    private Rigidbody carRb;


    void Awake()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = _centerOfMass;
    }

    void Update()
    {
        GetInputs();
        AnimateWheels();
        UpdateEngineSound();
    }

    void LateUpdate()
    {
        Move();
        Steer();
        Brake();
        TurnDireksiyon();

         
    }
    void TurnDireksiyon()
    {
        var _steerAngle = steerInput * turnSensitivity * maxSteerAngle;
        direksiyon.localRotation = Quaternion.Euler(90, _steerAngle, 0);
    }

    void GetInputs()
    {
        if(control == ControlMode.Keyboard)
        {
            moveInput = Input.GetAxis("Vertical");
            steerInput = Input.GetAxis("Horizontal");
        }
    }

    void Move()
    {
        foreach(var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = moveInput * 600 * maxAcceleration * Time.deltaTime;
        }
    }

    void Steer()
    {
        foreach(var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var _steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                Debug.Log(_steerAngle);
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, 0.6f);
            }
        }
    }

    void Brake()
    {
        if ((Input.GetKey(KeyCode.Space) || isVrBraking )|| moveInput == 0)
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 300 * brakeAcceleration * Time.deltaTime;
            }
        }
        else
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0;
            }
        }
    }

    void AnimateWheels()
    {
        foreach(var wheel in wheels)
        {
            Quaternion rot;
            Vector3 pos;
            wheel.wheelCollider.GetWorldPose(out pos, out rot);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = rot;
        }
    }

    void UpdateEngineSound()
    {
        // Calculate the average RPM of the wheels
        float totalRPM = 0;
        foreach (var wheel in wheels)
        {
            totalRPM += wheel.wheelCollider.rpm;
        }
        float averageRPM = totalRPM / wheels.Count;

        // Normalize the RPM value (this range might need tuning based on the actual game)
        float normalizedRPM = Mathf.InverseLerp(0, maxRPMForVolume, Mathf.Abs(averageRPM));

        // Set the audio volume based on the normalized RPM
        audioSource.volume = normalizedRPM;
    }
    
}