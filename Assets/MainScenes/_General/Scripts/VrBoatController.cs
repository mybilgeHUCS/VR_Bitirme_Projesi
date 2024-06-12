using BoatAttack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class VrBoatController : MonoBehaviour
{
    
    private InputData _inputData;
    public HumanController boatHumanController;
     private void Awake()
    {
        _inputData = GetComponent<InputData>();
    }

    private void Start()
    {
        boatHumanController = GameObject.FindObjectOfType<HumanController>();
        if (boatHumanController == null && SceneManager.GetActiveScene().name == "BoatTripScene")
        {
            Debug.Log("HumanController bulunamadi");
        }
    }

    private void Update() {

        if(boatHumanController == null){
            return;
        }

        _inputData._rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightControllerAxis);
        
        //Debug.Log("rightControllerAxisPrimary " + rightControllerAxis);

        //_inputData._rightController.TryGetFeatureValue(CommonUsages.secondary2DAxis, out Vector2 rightControllerAxisSecondary);
        
        //Debug.Log("rightControllerAxisSecondary " + rightControllerAxisSecondary);

        //_inputData._rightController.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButton);
        
        //Debug.Log("triggerButton " + triggerButton);


        boatHumanController._throttle = rightControllerAxis.y;
        boatHumanController._steering = rightControllerAxis.x;

    }
}
