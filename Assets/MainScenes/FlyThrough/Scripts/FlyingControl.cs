using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingControl : MonoBehaviour
{
    public float thrust;
    public float thrust_multiplier;
    public float yaw_multiplier;
    public float pitch_multiplier;
    new Rigidbody rigidbody;
    [SerializeField]  bool canKeyboardControl = false;

    public float pitch;
    public float yaw;
    float globalSpeedMultiplier;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        globalSpeedMultiplier = PlayerPrefs.GetFloat("SpeedMultiplierSlider");
    }

    void FixedUpdate()
    {
        //transform.position = new Vector3(transform.position.x, Math.Clamp(transform.position.y,10f,200f), transform.position.z);

        if(canKeyboardControl){
            pitch = Input.GetAxis("Vertical");
            yaw = Input.GetAxis("Horizontal");
        }
        


        float speed = thrust * thrust_multiplier * Time.deltaTime * globalSpeedMultiplier;


        rigidbody.velocity = transform.forward * speed;
        rigidbody.AddRelativeTorque(pitch * pitch_multiplier * Time.deltaTime,
                                   yaw * yaw_multiplier * Time.deltaTime,
                                   -yaw * yaw_multiplier * 2 * Time.deltaTime);

        //Debug.Log(rigidbody.velocity.magnitude);
    }
}
