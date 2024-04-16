using UnityEngine;

public class Mover : MonoBehaviour
{
    private float speed;

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime; // Move forward at the given speed
    }
}
