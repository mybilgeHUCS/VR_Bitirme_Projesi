using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VrFlyThroughController : MonoBehaviour
{
    
    private InputData _inputData;
    //[SerializeField] CarController carController;
     private void Awake()
    {
        _inputData = GetComponent<InputData>();
    }

    private void Update() {

        /*if(carController == null){
            return;
        }*/

        _inputData._rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightControllerAxis);
        
        Debug.Log("rightControllerAxisPrimary " + rightControllerAxis);

        _inputData._rightController.TryGetFeatureValue(CommonUsages.secondary2DAxis, out Vector2 rightControllerAxisSecondary);
        
        Debug.Log("rightControllerAxisSecondary " + rightControllerAxisSecondary);

        _inputData._rightController.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButton);
        
        Debug.Log("triggerButton " + triggerButton);

       
    }
}
