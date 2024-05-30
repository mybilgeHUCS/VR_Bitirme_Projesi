using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VrF1RaceController : MonoBehaviour
{
    private InputData _inputData;
    [SerializeField] CarController carController;

    private void Awake()
    {
        _inputData = GetComponent<InputData>();
    }

    private void Update() {


        if(carController == null){
            return;
        }

        _inputData._rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightControllerAxis);
        
        //Debug.Log("rightControllerAxisPrimary " + rightControllerAxis);

        _inputData._rightController.TryGetFeatureValue(CommonUsages.secondary2DAxis, out Vector2 rightControllerAxisSecondary);
        
        //Debug.Log("rightControllerAxisSecondary " + rightControllerAxisSecondary);

        _inputData._rightController.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButton);

        _inputData._rightController.TryGetFeatureValue(CommonUsages.trigger, out float triggerButtonFloat);


        _inputData._rightController.TryGetFeatureValue(CommonUsages.gripButton, out bool gripButton);

        _inputData._rightController.TryGetFeatureValue(CommonUsages.grip, out float gripButtonFloat);


        Debug.Log(triggerButtonFloat +" aaa "+ gripButtonFloat);


        if (carController.control == CarController.ControlMode.VR){
            //carController.moveInput = rightControllerAxis.y;
            carController.steerInput = rightControllerAxis.x;

            if (triggerButton)
            {
                carController.moveInput = triggerButtonFloat;
            }
            else if (gripButton)
            {
                carController.moveInput = -gripButtonFloat;
            }
            else if (!gripButton && !triggerButton)
            {
                carController.moveInput = 0f;
            }



            //carController.isVrBraking = triggerButton;
        }        
    }
}
