using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VrFlyThroughController : MonoBehaviour
{
    
    private InputData _inputData;
    [SerializeField] FlyingControl flyController;
     private void Awake()
    {
        _inputData = GetComponent<InputData>();
    }

    private void Update() {

        if(flyController == null){
            return;
        }

        _inputData._rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightControllerAxis);

        Debug.Log("rightControllerAxisPrimary " + rightControllerAxis);

        //_inputData._rightController.TryGetFeatureValue(CommonUsages.secondary2DAxis, out Vector2 rightControllerAxisSecondary);
        
        //Debug.Log("rightControllerAxisSecondary " + rightControllerAxisSecondary);

        _inputData._rightController.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButton);

        _inputData._rightController.TryGetFeatureValue(CommonUsages.trigger, out float triggerButtonFloat);


        _inputData._rightController.TryGetFeatureValue(CommonUsages.gripButton, out bool gripButton);

        _inputData._rightController.TryGetFeatureValue(CommonUsages.grip, out float gripButtonFloat);

        Debug.Log("triggerButton " + triggerButtonFloat);
        Debug.Log("gripButton " + gripButtonFloat);

        if (triggerButton)
        {
            flyController.pitch = triggerButtonFloat;
        }
        else if (gripButton)
        {
            flyController.pitch = -gripButtonFloat;
        }
        else if (!gripButton && !triggerButton)
        {
            flyController.pitch = 0f;
        }

        //flyController.pitch = rightControllerAxis.y;


        flyController.yaw = rightControllerAxis.x;

    }
}
