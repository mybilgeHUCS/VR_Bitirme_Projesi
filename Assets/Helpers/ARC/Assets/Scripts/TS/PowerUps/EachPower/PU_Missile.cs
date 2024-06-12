// Description: PU_Missile: Called by PowerUpsSystemAssisatnt on vehicle.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    [System.Serializable]
    public class PU_Missile
    {
        public AudioClip                missileSound;
        public float                    aVolume = 1;
        public bool                     b_IsMissileActivated;
        public int                      howManyMissiles = 3;
        public int                      HowManyTargets = 3;
        public int                      lifeCost = -2;

        public float                    detectedTargetMinDistance = 25;
        public float                    detectedTargetMaxDistance = 170;

        [HideInInspector]
        public int                      currentMissileNumber;

        public GameObject               missilePrefab;
        public List<Transform>          instantiatePosition = new List<Transform>();
        public int                      currentInstantiatePos;

        public float                    fireRate = .15f;

        public PowerUpsAIDetectVehicle  DetectVehicleFront;
        public bool                     b_Is_Missile_Available = true;
        private PowerUpsSystem          powerUpsSystem;

        private bool                    b_IsInitDone;
        private bool                    b_AI;

        public List<GameObject>         missileFollowThisVehicle = new List<GameObject>();
        public bool                     isMissileLoked;

        //-> Player P1 | P2
        public IEnumerator InstantiateMissilePrefabRoutine(AudioSource aSource, int whichCamToCheck,MonoBehaviour mono)
        {
            List<Transform> visibleEnemies = returnVisibleEnemies(whichCamToCheck);
            PowerUpsSystem powerUpsSystem = mono.GetComponent<PowerUpsSystem>();

            for (var i = 0; i < PowerUpsSceneRef.instance.listUIMissileTargetsByPlayer[whichCamToCheck].listUIMissileTargets.Count; i++)
            {
                if (PowerUpsSceneRef.instance.listUIMissileTargetsByPlayer[whichCamToCheck].listUIMissileTargets[i].activeSelf)
                    PowerUpsSceneRef.instance.listUIMissileTargetsByPlayer[whichCamToCheck].listUIMissileTargets[i].SetActive(false);
            }

            for (var i = 0; i < howManyMissiles; i++)
            {

                if (visibleEnemies.Count > 0)
                    instantiateMissile(aSource, visibleEnemies[i % visibleEnemies.Count], whichCamToCheck, mono, powerUpsSystem.Grp_PowerUps);
                else
                    instantiateMissile(aSource, null, whichCamToCheck, mono, powerUpsSystem.Grp_PowerUps);

                 float t = 0;

                 while (t != fireRate)
                 {
                     if (!PauseManager.instance.Bool_IsGamePaused)
                         t = Mathf.MoveTowards(t, fireRate, Time.deltaTime);
                     yield return null;
                 }
            }

            DisableMissilePowerUp(whichCamToCheck);
            yield return null;
        }

        void instantiateMissile(AudioSource aSource, Transform visibleEnemy, int whichCamToCheck,MonoBehaviour mono,Transform parentPos)
        {
            
            GameObject newMissile = GameObject.Instantiate(
                 missilePrefab,
                 instantiatePosition[currentInstantiatePos].position,
                 instantiatePosition[currentInstantiatePos].rotation,
                 parentPos);

            newMissile.transform.position = instantiatePosition[currentInstantiatePos].position;

            PU_MissilePrefab pU_MissilePrefab = newMissile.GetComponent<PU_MissilePrefab>();
            pU_MissilePrefab.Init(visibleEnemy, whichCamToCheck, mono, lifeCost);

            if (missileSound && aSource && aSource.gameObject.activeInHierarchy)
            {
                aSource.clip = missileSound;
                aSource.volume = aVolume;
                aSource.Play();
            }

            currentInstantiatePos++;
            currentInstantiatePos %= instantiatePosition.Count;
            currentMissileNumber--;
        }

        public void InitMissilePowerUp(bool b_RefAI, PowerUpsSystem upsSystem)
        {
            if (!powerUpsSystem) powerUpsSystem = upsSystem;

            // Init Power Up when the scene is loaded
            if (b_IsInitDone)
            {
                b_AI = b_RefAI;
                //-> P1 | P2 only
                if (!b_AI)
                {
                    //-> Case P1 P2: Select the powerups detector contained into the Cam P1 or P2
                    Cam_Follow[] playerCameras = GameObject.FindObjectsOfType<Cam_Follow>();

                    foreach (Cam_Follow cam in playerCameras)
                    {
                        if (cam.playerID == powerUpsSystem.vehicleInfo.playerNumber)
                        {
                            DetectVehicleFront = cam.EnemyDetector_01_Front;
                        }
                    }
                }
            }
            // Init Power Up during the game
            else
            {

            }

            currentInstantiatePos = 0;
            currentMissileNumber = howManyMissiles;
            b_Is_Missile_Available = true;
            //Debug.Log("Missile Init");
        }

        public void DisableMissilePowerUp(int whichCamToCheck)
        {
            currentMissileNumber = 0;
            b_Is_Missile_Available = false;

            int howManyPlayer = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer;

            if(howManyPlayer > whichCamToCheck)
            {
                for (var i = 0; i < PowerUpsSceneRef.instance.listUIMissileTargetsByPlayer[whichCamToCheck].listUIMissileTargets.Count; i++)
                {
                    PowerUpsSceneRef.instance.listUIMissileTargetsByPlayer[whichCamToCheck].listUIMissileTargets[i].SetActive(false);
                }
            }

            for(var i = 0; i < instantiatePosition.Count; i++)
            {
                for (var j = 0; j < instantiatePosition[i].childCount; j++)
                {
                    instantiatePosition[i].GetChild(j).transform.SetParent(null);
                }
            }
        }

        public void DisplayUIMissileTargets(int whichCamToCheck)
        {
            int howManyPlayer = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer;
            //Debug.Log("whichCamToCheck -> " + whichCamToCheck);
            if (howManyPlayer > whichCamToCheck)
            {
                List<Transform> visibleEnemies = returnVisibleEnemies(whichCamToCheck);

                for (var i = 0; i < PowerUpsSceneRef.instance.listUIMissileTargetsByPlayer[whichCamToCheck].listUIMissileTargets.Count; i++)
                {
                    if (i < visibleEnemies.Count)
                    {
                        Vector3 screenPos = CamRef.instance.listCameras[whichCamToCheck].WorldToScreenPoint(visibleEnemies[i].position);

                        if (!PowerUpsSceneRef.instance.listUIMissileTargetsByPlayer[whichCamToCheck].listUIMissileTargets[i].activeSelf)
                            PowerUpsSceneRef.instance.listUIMissileTargetsByPlayer[whichCamToCheck].listUIMissileTargets[i].SetActive(true);

                        PowerUpsSceneRef.instance.listUIMissileTargetsByPlayer[whichCamToCheck].listUIMissileTargets[i].transform.position = screenPos;
                    }
                    else
                    {
                        if (PowerUpsSceneRef.instance.listUIMissileTargetsByPlayer[whichCamToCheck].listUIMissileTargets[i].activeSelf)
                            PowerUpsSceneRef.instance.listUIMissileTargetsByPlayer[whichCamToCheck].listUIMissileTargets[i].SetActive(false);
                    }
                }
            }
        }


        List<Transform> returnVisibleEnemies(int whichCamToCheck)
        {
            if (b_Is_Missile_Available)
            {
                List<Transform> listVehicleVisible = new List<Transform>();
                VehiclesVisibleByTheCamList listVehicles = VehiclesVisibleByTheCamList.instance;

                for (var i = 0; i < listVehicles.listVehicles.Count; i++)
                {
                    bool b_Visible = listVehicles.listVehiclesVisibleByCamera[whichCamToCheck].listVehiclesVisible[i];

                    float distance = Vector3.Distance(powerUpsSystem.transform.position, listVehicles.listVehicles[i].transform.position);

                    if (listVehicles.listVehicles[i] &&
                        distance > detectedTargetMinDistance &&
                        distance < detectedTargetMaxDistance &&
                        b_Visible &&
                        listVehicleVisible.Count < HowManyTargets &&
                        !listVehicles.listVehicles[i].GetComponent<VehicleDamage>().b_Invincibility/* &&
                        !listVehicles.listVehicles[i].GetComponent<VehicleDamage>().b_StartLineInvincibility*/)
                        listVehicleVisible.Add(listVehicles.listVehicles[i].transform);
                }
                return listVehicleVisible;
            }

            return null;
        }

        //-> AI launches missiles
        public IEnumerator MissileAIRoutine(AudioSource aSource, int whichCamToCheck, MonoBehaviour mono)
        {
            if (DetectVehicleFront && DetectVehicleFront.b_VehicleEnemyDetected && b_Is_Missile_Available
                ||
                b_Is_Missile_Available)
            {
                b_Is_Missile_Available = false;


                bool stopProcess = false;

                //GameObject lastVehicleDetected = DetectVehicleFront.currentPlaneDetected;

                for (var i = 0; i < 2; i++)
                {
                    if (DetectVehicleFront.currentPlaneDetected)
                    {
                        PowerUpsSystem powerUpsSystem = DetectVehicleFront.currentPlaneDetected.transform.parent.GetComponent<PowerUpsSystem>();
                        VehicleDamage vehicleDamage = DetectVehicleFront.currentPlaneDetected.transform.parent.GetComponent<VehicleDamage>();
                        if (!powerUpsSystem.b_EnemyAttackAllowed || vehicleDamage.b_Invincibility/* || vehicleDamage.b_StartLineInvincibility*/)
                        {
                            stopProcess = true;
                        }
                        else
                        {
                            float t = 0;

                            

                            if (vehicleDamage.b_Invincibility)
                                t = 1;
                            else
                                powerUpsSystem.PlayerLockedWarning(159, new Color(1,.5f,0,.55f));    // !!! Missile !!!

                            while (t < 1)
                            {
                                if (!PauseManager.instance.Bool_IsGamePaused)
                                {
                                    t += Time.deltaTime;

                                    if (vehicleDamage.b_Invincibility/* || vehicleDamage.b_StartLineInvincibility*/)
                                        t = 1;
                                }
                                yield return null;
                            }
                        }

                        if (!powerUpsSystem.b_EnemyAttackAllowed || vehicleDamage.b_Invincibility/* || vehicleDamage.b_StartLineInvincibility*/)
                        {
                            stopProcess = true;
                        }
                    }
                }

                //Debug.Log("stopProcess: " + stopProcess + " | DetectVehicleFront.b_VehicleEnemyDetected ->" + DetectVehicleFront.b_VehicleEnemyDetected);

                //-> If the enemy still detected after 2s launch the missile
                // + Check if the vehicle can be attacked (stopProcess)
                if (DetectVehicleFront.b_VehicleEnemyDetected && !stopProcess)
                {
                    PowerUpsSystem powerUpsSystem = mono.GetComponent<PowerUpsSystem>();
                    //Debug.Log("visibleEnemies: " + visibleEnemies.Count);

                    for (var i = 0; i < howManyMissiles; i++)
                    {
                        if (DetectVehicleFront.currentPlaneDetected)
                            instantiateMissile(aSource, DetectVehicleFront.currentPlaneDetected.transform, whichCamToCheck, mono, powerUpsSystem.Grp_PowerUps);
                        else
                            instantiateMissile(aSource, null, whichCamToCheck, mono, powerUpsSystem.Grp_PowerUps);

                        float t = 0;

                        while (t != fireRate)
                        {
                            if (!PauseManager.instance.Bool_IsGamePaused)
                            {
                                t = Mathf.MoveTowards(t, fireRate, Time.deltaTime);
                            }
                            yield return null;
                        }
                    }

                    DisableMissilePowerUp(whichCamToCheck);
                }
                else
                {
                    b_Is_Missile_Available = true;
                }
            }
           
            yield return null;
        }


        public void AddToMissileToTheList(GameObject missile)
        {
            missileFollowThisVehicle.Add(missile);
            if (!isMissileLoked)
            {
                powerUpsSystem.StartCoroutine(WaitUntilAllMissileDestroyed());
            }
        }

        

        public IEnumerator WaitUntilAllMissileDestroyed()
        {
            isMissileLoked = true;
            
            float t = 0;
            int counter = 0;
            while (missileFollowThisVehicle.Count > 0)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
                    for(var i = 0;i < missileFollowThisVehicle.Count; i++)
                    {
                        if (!missileFollowThisVehicle[i] || !missileFollowThisVehicle[i].transform.GetChild(0).gameObject.activeInHierarchy)
                            missileFollowThisVehicle.RemoveAt(i);
                    }

                    if (t == 0 && counter == 1)
                    {
                        powerUpsSystem.StartCoroutine(powerUpsSystem.PlayerLockedWarningRoutine(161, new Color(1, 0, 0, .55f))); 
                    }
                    t += Time.deltaTime;

                    if (t > .25f)
                    {
                        t = 0;
                        counter++;
                        counter %= 2;
                        if(!powerUpsSystem.vehicleAI.enabled)
                            powerUpsSystem.objPlayerLocked.SetActive(false);
                    }

                    if (!b_Is_Missile_Available)
                        missileFollowThisVehicle.Clear();

                }
                yield return null;
            }

            missileFollowThisVehicle.Clear();
            isMissileLoked = false;
            yield return null;
        }
    }
}