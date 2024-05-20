using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LunaparkSceneManager : MonoBehaviour
{
    // Array to hold the cameras
    public GameObject[] cameras = new GameObject[13];

    // Start is called before the first frame update
    void Awake()
    {
        // Initialize by deactivating all cameras except the first one
        for (int i = 1; i < cameras.Length; i++)
        {
            if (cameras[i] != null)
                cameras[i].gameObject.SetActive(false);
        }

        //if (cameras[0] != null)
          //  cameras[0].gameObject.SetActive(true);
        SetActiveCamera(PlayerPrefs.GetInt("CameraNumber"));
    }

    // Public function to set a specific camera active
    public void SetActiveCamera(int index)
    {
        if (index < 0 || index >= cameras.Length)
        {
            Debug.LogError("Camera index out of range");
            return;
        }

        // Deactivate all cameras
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] != null)
                cameras[i].gameObject.SetActive(false);
        }

        // Activate the selected camera
        if (cameras[index] != null)
        {
            cameras[index].gameObject.SetActive(true);
        }
            
    }
    
}
