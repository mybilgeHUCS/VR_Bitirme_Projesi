using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

namespace TS.Generics
{
    public class VehiclePrefabPauseAssistant : MonoBehaviour
    {
        public Rigidbody    vRb;
        public bool         remKinematicState;
        public Vector3      remVelocity;

       public bool PauseVehicle()
        {
            //Debug.Log("Pause Vehicle");
            return true;
        }

        public bool UnPauseVehicle()
        {
            //Debug.Log("UnPause Vehicle");
            return true;
        }



        public bool PauseMovement()
        {
            remKinematicState = vRb.isKinematic;
            remVelocity = vRb.velocity;
            vRb.isKinematic = true;
            vRb.velocity = Vector3.zero;
            
            return true;
        }

        public bool UnpauseMovement()
        {
            vRb.isKinematic = remKinematicState;
            vRb.velocity = remVelocity;
            return true;
        }
    }
}