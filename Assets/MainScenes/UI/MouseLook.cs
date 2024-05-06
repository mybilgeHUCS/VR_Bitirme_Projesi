using UnityEngine;

public class CustomCursorLock : MonoBehaviour
{
    public float mouseSensitivity = 100.0f;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    void Start()
    {
        // Optionally hide the real cursor if you want
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        yRotation += mouseX;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Prevent flipping over

        transform.eulerAngles = new Vector3(xRotation, yRotation, 0.0f);

        // Reset cursor to the center of the screen
        Cursor.SetCursor(null, new Vector2(Screen.width / 2, Screen.height / 2), CursorMode.Auto);
    }
}
