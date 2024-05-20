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

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float pitch = Input.GetAxis("Vertical");
        float yaw = Input.GetAxis("Horizontal");

        rigidbody.AddRelativeForce(0f, 0f, thrust * thrust_multiplier * Time.deltaTime);
        rigidbody.AddRelativeTorque(pitch * pitch_multiplier * Time.deltaTime,
                                   yaw * yaw_multiplier * Time.deltaTime,
                                   -yaw * yaw_multiplier * 2 * Time.deltaTime);
    }
}
