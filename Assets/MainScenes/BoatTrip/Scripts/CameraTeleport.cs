using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CameraTeleport : MonoBehaviour
{
    private InputData _inputData;
    [SerializeField] GameObject player;
    [SerializeField] Transform[] teleportPositons;
    int index;
    bool canTeleport = true;
     private void Awake()
    {
        _inputData = player.GetComponentInChildren<InputData>();
    }

    private void Update() {

        _inputData._rightController.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButton);
        
        Debug.Log("triggerButton " + triggerButton);

        if((Input.GetKeyDown(KeyCode.T) || triggerButton) && canTeleport){
            //teleport
            player.transform.position = teleportPositons[++index% teleportPositons.Length].position;
            canTeleport = false;
        }

        if(Input.GetKeyUp(KeyCode.T) || !triggerButton){
            canTeleport = true;
        }



    }
}
