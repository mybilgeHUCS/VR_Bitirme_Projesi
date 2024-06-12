using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TS.Generics;

namespace TS.Generics
{
    public class VehicleInfo : MonoBehaviour
    {
        public bool                     b_InitDone;
        private bool                    b_InitInProgress;

        [Header("Player ID")]
        public int                      playerNumber;
        public bool                     b_IsVehicleAvailableToMove = true;
        public bool                     b_IsPlayerInputAvailable = true;        // Useful during the countdown to diable the player to move the plane

        [Header("Respawn")]
        public bool                     b_IsRespawn;                            // Know if the plane needs to be repawned

        

        //-> Initialisation
        public bool bInitVehicleInfo()
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(InitVehicleInfoRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;

            return b_InitDone;
            #endregion
        }


        IEnumerator InitVehicleInfoRoutine()
        {
            #region
           
            if(transform.parent.GetComponent<VehiclePrefabInit>())
                playerNumber = transform.parent.GetComponent<VehiclePrefabInit>().playerNumber;


            /*if (VehiclesRef.instance)
            {
                VehiclesRef.instance.listVehicles.Add(gameObject.GetComponent<VehicleInfo>());
            }*/

            //Debug.Log("Init: VehicleInfo -> Done");
            b_InitDone = true;
            yield return null;
            #endregion
        }

        public void DisableAIDuringInitialisationIfNeeded()
        {
            if (playerNumber == 0
                ||
                playerNumber == 1 && InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer == 2)
            {
                GetComponent<VehicleAI>().enabled = false;
            }
        }
    }

}
