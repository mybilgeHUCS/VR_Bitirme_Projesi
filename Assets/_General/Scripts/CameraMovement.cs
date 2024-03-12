using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    float rotationX = 0f;
    float rotationY = 0f;
 
    public Vector2 sensitivity = Vector2.one * 360f;

    private void Start() {
        Cursor.visible = false;
    }
 
    void Update()
    {

        if(Input.GetMouseButtonDown(0)){
            Cursor.visible = false;
        }
        rotationY += Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity.x;
        rotationX += Input.GetAxis("Mouse Y") * Time.deltaTime * -1 * sensitivity.y;
        transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
    }
}
