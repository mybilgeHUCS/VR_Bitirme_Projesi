// Description: PowerUpsSystem. This script manage power-ups for each vehicle

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace TS.Generics {
    public class PowerUpsSystem : MonoBehaviour
    {
        [HideInInspector]
        public bool SeeInspector;
        [HideInInspector]
        public bool moreOptions;
        [HideInInspector]
        public bool helpBox = true;
        [HideInInspector]
        public int editorTab;

        public bool b_InitDone;
        [HideInInspector]
        public bool b_InitInProgress;
        [HideInInspector]
        public VehiclePrefabInit vehiclePrefabInit;

        [Header("For All Power-Ups")]
        //public bool b_AI = false;
        public int currentPowerUps;
        public GameObject Grp_EnemyDetector;

        [HideInInspector]
        public VehicleDamage vehicleDamage;
        [HideInInspector]
        public VehicleInfo vehicleInfo;
        [HideInInspector]
        public VehicleBooster vehicleBooster;
        [HideInInspector]
        public VehicleAI vehicleAI;
        [HideInInspector]
        public VehicleInputs vehicleInputs;

        [HideInInspector]
        public bool b_IsKeyPressed;
        [HideInInspector]
        public bool b_IsKeyPressedDown;
        [HideInInspector]
        public bool b_IsKeyPressedUp;

        public AudioSource aSourcePowerUps;
        public AudioSource aSourceWarning;

        public float distancePowerUpsDetection = 200;

        public List<int> listLayersUsedBylayerMaskPowerDetection = new List<int>();
        [HideInInspector]
        public LayerMask layerMaskPowerDetection;
        [HideInInspector]
        public bool b_IsPowerUpDetected;

        public bool b_PowerUpsAllowed = true;

        [HideInInspector]
        public GameObject objPlayerLocked;

        public bool bKeepSelectedPowerUp = true;

        public Transform Grp_PowerUps;

        [HideInInspector]
        public bool b_EnemyAttackAllowed = true;                  // if false the enemies can't shoot the player
        [HideInInspector]
        public float timerEnemyAttackAllowed = 0;
        [HideInInspector]
        public int howManyAttackDetected;
        public int howManyAttackAllowed =3;
        public float timerDuration = 10;

        [HideInInspector]
        public float delayBeforeActivation = 0;

        [HideInInspector]
        public bool b_IsDelayComplete = true;                             // Vehicle is allowed to attack only if b_IsDelayComplete = true

        [System.Serializable]
        public class PowerUpInit
        {
            public string name;
            public GameObject objSelectionsRules;
            public int ID;
        }




        // Init Power-up UI
        [HideInInspector]
        public List<PowerUpInit> puInitUIList = new List<PowerUpInit>();
        private List<IPUSystemUIInit<PUInfo>> powerUpUIInit = new List<IPUSystemUIInit<PUInfo>>();

        // Init All Power-up
        [HideInInspector]
        public List<PowerUpInit> objPUInitList = new List<PowerUpInit>();
        private List<IPowerUpSystemInit<PUInfo>> powerUpInit = new List<IPowerUpSystemInit<PUInfo>>();

        // Disable All Power-up
        [HideInInspector]
        public List<PowerUpInit> puDisableList = new List<PowerUpInit>();
        private List<IPUSystemDisable<PUInfo>> powerUpDisable = new List<IPUSystemDisable<PUInfo>>();

        // Update AI Power-up
        [HideInInspector]
        public List<PowerUpInit> puAIUpdateList = new List<PowerUpInit>();
        private List<IPUSysUpdateAI<PUInfo>> powerUpAIUpdate = new List<IPUSysUpdateAI<PUInfo>>();

        // Update Player Power-up
        [HideInInspector]
        public List<PowerUpInit> puplayerUpdateList = new List<PowerUpInit>();
        private List<IPUSysUpdateplayer<PUInfo>> powerUpPlayerUpdate = new List<IPUSysUpdateplayer<PUInfo>>();

        // OnTriggerEnter Power-up
        [HideInInspector]
        public List<PowerUpInit> puOnTriggerEnterList = new List<PowerUpInit>();
        private List<IPUSysOnTriggerEnter<PUInfo>> powerUpOnTriggerEnter = new List<IPUSysOnTriggerEnter<PUInfo>>();

        [System.Serializable]
        public class PowerUpChangePowerUpRules
        {
            public string name;
            public GameObject objSelectionsRules;
            public int ID;
        }

        // Allow to change Power-up (Rules)
        [HideInInspector]
        public List<PowerUpChangePowerUpRules> puAllowToChangePUList = new List<PowerUpChangePowerUpRules>();
        private List<IPUAllowToChangePU<PUAllowChange>> powerAllowToChangePU = new List<IPUAllowToChangePU<PUAllowChange>>();


        public Action<int> NewPowerUpSelected;


        // Start is called before the first frame update
        void Start()
        {
            //-> Init LayerMask
            string[] layerUsed = new string[listLayersUsedBylayerMaskPowerDetection.Count];
            for (var i = 0; i < listLayersUsedBylayerMaskPowerDetection.Count; i++)
                layerUsed[i] = LayerMask.LayerToName(LayersRef.instance.layersListData.listLayerInfo[listLayersUsedBylayerMaskPowerDetection[i]].layerID);
            layerMaskPowerDetection = LayerMask.GetMask(layerUsed);

            vehicleAI = GetComponent<VehicleAI>();
            vehicleInfo = GetComponent<VehicleInfo>();

            vehicleDamage = GetComponent<VehicleDamage>();
            vehicleDamage.VehicleExplosionAction += VehicleExplosion;
            vehicleDamage.VehicleRespawnPart2 += VehicleRespawn;

            vehicleBooster = GetComponent<VehicleBooster>();

            vehicleInputs = GetComponent<VehicleInputs>();
            vehicleInputs.PowerUpGetKeyDown += PowerUpGetKeyDown;
            vehicleInputs.PowerUpGetKeyUp += PowerUpGetKeyUp;
        }

        //-> Initialisation
        public bool bInitPowerUpsSystem()
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(InitRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;

            return b_InitDone;
            #endregion
        }

        IEnumerator InitRoutine()
        {
            //-> Power-up UI Init List: Init
            for (var i = 0; i < puInitUIList.Count; i++)
            {
                powerUpUIInit.Add(puInitUIList[i].objSelectionsRules.GetComponent<IPUSystemUIInit<PUInfo>>());
            }

            //-> Power-up Init All PowerList: Init
            for (var i = 0; i < objPUInitList.Count; i++)
            {
                powerUpInit.Add(objPUInitList[i].objSelectionsRules.GetComponent<IPowerUpSystemInit<PUInfo>>());
            }

            //-> All Power-up: Disable
            for (var i = 0; i < puDisableList.Count; i++)
            {
                powerUpDisable.Add(puDisableList[i].objSelectionsRules.GetComponent<IPUSystemDisable<PUInfo>>());
            }

            //-> AI Power-up: Update
            for (var i = 0; i < puAIUpdateList.Count; i++)
            {
                powerUpAIUpdate.Add(puAIUpdateList[i].objSelectionsRules.GetComponent<IPUSysUpdateAI<PUInfo>>());
            }

            //-> Player Power-up: Update
            for (var i = 0; i < puplayerUpdateList.Count; i++)
            {
                powerUpPlayerUpdate.Add(puplayerUpdateList[i].objSelectionsRules.GetComponent<IPUSysUpdateplayer<PUInfo>>());
            }

            //-> Power-up: OnTriggerEnter
            for (var i = 0; i < puOnTriggerEnterList.Count; i++)
            {
                powerUpOnTriggerEnter.Add(puOnTriggerEnterList[i].objSelectionsRules.GetComponent<IPUSysOnTriggerEnter<PUInfo>>());
            }

            //->  Allow to change Power-up (Rules)
            for (var i = 0; i < puAllowToChangePUList.Count; i++)
            {
                powerAllowToChangePU.Add(puAllowToChangePUList[i].objSelectionsRules.GetComponent<IPUAllowToChangePU<PUAllowChange>>());
            }


            vehiclePrefabInit = transform.parent.GetComponent<VehiclePrefabInit>();
            int howManyPlayer = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer;

            //-> P1 | P2 only
            if (vehicleInfo.playerNumber == 0 && howManyPlayer > 0 || vehicleInfo.playerNumber == 1 && howManyPlayer > 1)
            {
                //-> Case P1 P2: Select the powerups detector contained into the Cam P1 or P2
                Cam_Follow[] playerCameras = GameObject.FindObjectsOfType<Cam_Follow>();

                foreach (Cam_Follow cam in playerCameras)
                {
                    if (cam.playerID == vehicleInfo.playerNumber)
                    {
                        Grp_EnemyDetector.SetActive(false);

                        Grp_EnemyDetector = cam.Grp_EnemyDetector;
                        cam.Grp_EnemyDetector.SetActive(true);
                    }
                }


                //-> init Power-up UI using InitPowerUpUI<PUInfo> Interface
                for (var i = 0; i < powerUpUIInit.Count; i++)
                {
                    PUInfo puInfo = new PUInfo(this, puInitUIList[i].ID);
                    powerUpUIInit[i].InitPowerUpUI(puInfo);
                }


                //-> Connect a UI objPlayerLocked (Display UI Warning when player is locked by an enemy)
                objPlayerLocked = CanvasInGameUIRef.instance.listPlayerUIElements[vehicleInfo.playerNumber].listRectTransform[0].gameObject;
            }


            //-> AI only
            if ((vehicleInfo.playerNumber != 0 && howManyPlayer > 0
                ||
                vehicleInfo.playerNumber == 1 && howManyPlayer == 1) &&
                vehicleInfo.playerNumber > 1)
            {
                //-> Delay Before Activation
                int[] delayValues = new int[4] { 5, 10, 15, 20 };
                int _rand = UnityEngine.Random.Range(0, delayValues.Length);
                delayBeforeActivation = delayValues[_rand];
                StartCoroutine(DelayBeforeActivationRoutine());
            }


            //-> All vehicle
            initAllPowerUps();


            b_InitDone = true;
            //Debug.Log("Init: PowerUpsSystem -> Done");
            yield return null;
        }

        public void OnDestroy()
        {
            vehicleDamage.VehicleExplosionAction -= VehicleExplosion;
            vehicleDamage.VehicleRespawnPart2 -= VehicleRespawn;

            vehicleInputs.PowerUpGetKeyDown -= PowerUpGetKeyDown;
            vehicleInputs.PowerUpGetKeyUp -= PowerUpGetKeyUp;
        }

        void initAllPowerUps()
        {
            //-> init Power-up using IPowerUpSystemInit<PUInfo> Interface
            for (var i = 0; i < powerUpInit.Count; i++)
            {
                PUInfo puInfo = new PUInfo(this, objPUInitList[i].ID);
                powerUpInit[i].InitPowerUp(puInfo);
            }

            NewPowerUp();
        }

        public void DisableAllPowerUps()
        {
            //-> init Power-up using IPowerUpSystemInit<PUInfo> Interface
            for (var i = 0; i < puDisableList.Count; i++)
            {
                PUInfo puInfo = new PUInfo(this, puDisableList[i].ID);
                powerUpDisable[i].DisablePowerUp(puInfo);
            }
        }

        public void PowerUpGetKeyDown()
        {
            b_IsKeyPressedDown = true;
            b_IsKeyPressedUp = false;
        }

        public void PowerUpGetKeyUp()
        {

            b_IsKeyPressedDown = false;
            b_IsKeyPressedUp = true;
            b_IsKeyPressed = false;
        }


        // Update is called once per frame
        void Update()
        {
            if (b_InitDone &&
                b_IsDelayComplete &&
                vehiclePrefabInit &&
                vehiclePrefabInit.b_InitDone &&
                !PauseManager.instance.Bool_IsGamePaused &&
                LapCounterAndPosition.instance.posList.Count > vehicleInfo.playerNumber &&
                !LapCounterAndPosition.instance.posList[vehicleInfo.playerNumber].IsRaceComplete)
            {
                if (vehicleAI.enabled)
                {
                    RaycastPowerUpFront();
                    AICases();
                }
                else
                {
                    PlayerCases();
                }
            }
        }

        //-> Enable the current power-up if the vehicle facing a power-up
        void RaycastPowerUpFront()
        {
            RaycastHit hit;
            if (Physics.Raycast(Grp_EnemyDetector.transform.transform.position, Grp_EnemyDetector.transform.transform.forward, out hit, distancePowerUpsDetection, layerMaskPowerDetection))
            {
                Debug.DrawRay(Grp_EnemyDetector.transform.position, Grp_EnemyDetector.transform.forward * hit.distance, Color.green);
                b_IsPowerUpDetected = true;
            }
            else
            {
                Debug.DrawRay(Grp_EnemyDetector.transform.position, Grp_EnemyDetector.transform.forward * distancePowerUpsDetection, Color.red);
                b_IsPowerUpDetected = false;
            }
        }

        void AICases()
        {
            if(currentPowerUps != 0)
            {
                PUInfo puInfo = new PUInfo(this, currentPowerUps);
                powerUpAIUpdate[currentPowerUps].AIUpdatePowerUp(puInfo);
            }
        }
        
        void PlayerCases()
        {
            // If different: No Power-up or Random case
            if (currentPowerUps != 0 && currentPowerUps != 1000)
            {
                PUInfo puInfo = new PUInfo(this, currentPowerUps);
                //Debug.Log("currentPowerUps: " + currentPowerUps);
                powerUpPlayerUpdate[currentPowerUps].PlayerUpdatePowerUp(puInfo);
            }
        }


        // Feedback when player 1 or 2 are locked
        public void PlayerLockedWarning(int txtID, Color color)
        {
            if(howManyAttackDetected == 0)
                StartCoroutine(HowManyAttackDetectedRoutine());

            howManyAttackDetected++;

            if (!GetComponent<VehicleAI>().enabled &&
                !aSourceWarning.isPlaying &&
                !vehicleInfo.b_IsRespawn &&
                howManyAttackDetected < howManyAttackAllowed + 1)
            {
                StartCoroutine(PlayerLockedWarningRoutine(txtID, color));
            }
               
        }

        public IEnumerator HowManyAttackDetectedRoutine()
        {
            timerEnemyAttackAllowed = 0;
            while (timerEnemyAttackAllowed < timerDuration)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
                    timerEnemyAttackAllowed += Time.deltaTime;

                    if (howManyAttackDetected > howManyAttackAllowed)
                        timerEnemyAttackAllowed = timerDuration;
                }
                yield return null;
            }

            if (howManyAttackDetected > howManyAttackAllowed)
            {
                b_EnemyAttackAllowed = false;
                timerEnemyAttackAllowed = 0;
                while (timerEnemyAttackAllowed <= timerDuration)
                {
                    if (!PauseManager.instance.Bool_IsGamePaused)
                    {
                        timerEnemyAttackAllowed += Time.deltaTime;
                    }
                    yield return null;
                }
            }

            howManyAttackDetected = 0;
            b_EnemyAttackAllowed = true;

            yield return null;
        }

        public IEnumerator PlayerLockedWarningRoutine(int txtID, Color color)
        {
            Debug.Log("Warning");
            if (!vehicleAI.enabled)
            {
                if(aSourceWarning.gameObject.activeInHierarchy)
                    aSourceWarning.Play();
                objPlayerLocked.SetActive(true);

                CanvasInGameUIRef.instance.listPlayerUIElements[vehicleInfo.playerNumber].listTexts[3].NewTextWithSpecificID(txtID, 0);
                CanvasInGameUIRef.instance.listPlayerUIElements[vehicleInfo.playerNumber].listRectTransform[0].GetComponent<Image>().color = color;

                while (aSourceWarning.isPlaying)
                {
                    yield return null;
                }

                objPlayerLocked.SetActive(false);
            }
            yield return null;
        }



        void OnTriggerEnter(Collider other)
        {
            if (b_InitDone && vehiclePrefabInit && vehiclePrefabInit.b_InitDone)
            {
                //Debug.Log(other.transform.name);
                if (other.GetComponent<PowerUpsItems>() &&
                    b_PowerUpsAllowed &&
                    vehicleInfo.b_IsPlayerInputAvailable)
                {
                    #region
                    PowerUpsItems powerUpsItems = other.GetComponent<PowerUpsItems>();

                    if (CheckIfVehicleIsAllowedToChangeItsPowerUp(powerUpsItems))
                    {
                        b_IsKeyPressedDown = false;
                        b_IsKeyPressedUp = false;
                        DisableAllPowerUps();

                        currentPowerUps = powerUpsItems.PowerType;

                        //-> Init the position of the vehicle using its offset.
                        GetComponent<VehiclePathFollow>().NewOffsetTarget(Vector2.zero, true);

                        PUInfo puInfo = new PUInfo(this, currentPowerUps);
                        powerUpOnTriggerEnter[currentPowerUps].OnTriggerEnterPowerUp(puInfo);

                        // Disable Power-up trigger for a few seconds.
                        // Check if this vehicle is managed by the player 2 to spacial sound if needed
                        if (!vehicleAI.enabled && vehicleInfo.playerNumber == 1)
                            powerUpsItems.PowerUpItemActivated(true) ;
                        else
                            powerUpsItems.PowerUpItemActivated(false);
                    }
                    #endregion
                }
            }
        }

        public void VehicleExplosion()
        {
            NewPowerUp();

            b_IsKeyPressed = false;
            b_IsKeyPressedDown = false;
            b_IsKeyPressedUp = false;

            Grp_EnemyDetector.SetActive(false);

            // Disable all the other power-ups
            DisableAllPowerUps();

            StartCoroutine(vehicleDamage.InvincibiltyRoutine(Grp_EnemyDetector));
        }

        public void VehicleRespawn()
        {
            howManyAttackDetected = 0;
            b_EnemyAttackAllowed = true;
            Grp_EnemyDetector.SetActive(true);
        }

        public void NewPowerUp(int newPowerUp = 0)
        {
            currentPowerUps = newPowerUp;
            for (var i = 0; i < 2; i++)
            {
                if (vehicleInfo.playerNumber == i)
                {
                    if (PowerUpsSceneRef.instance.listUIPowerUpIcons[i])
                    {
                        PowerUpsSceneRef.instance.listUIPowerUpIcons[i].sprite =
                            PowerUpsSceneRef.instance.powerUpsDatas.listPowerUps[newPowerUp].spPowerUp;
                    }
                }
            }
            NewPowerUpSelected?.Invoke(newPowerUp);
        }

        bool CheckIfVehicleIsAllowedToChangeItsPowerUp(PowerUpsItems powerUpsItems)
        {
            //-> Check If Vehicle Is Allowed To Change Its Power-Up using IPUAllowToChangePU<PUInfo> Interface
            for (var i = 0; i < powerAllowToChangePU.Count; i++)
            {
                PUAllowChange puAllowChange = new PUAllowChange(this, powerUpsItems, puAllowToChangePUList[i].ID);
                //Debug.Log(powerAllowToChangePU.Count + "| i: " + i);
                bool b_Allow = powerAllowToChangePU[i].AllowToChangePowerUp(puAllowChange);
                if (b_Allow) return b_Allow;
            }
    
            return false;
        }

        // Delay: AI is allowed to use Power-up after this delay. Used when the race starts.
        public IEnumerator DelayBeforeActivationRoutine()
        {
            b_IsDelayComplete = false;
            float t = 0;
            while (t < delayBeforeActivation)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
                    t += Time.deltaTime;
                }
                yield return null;
            }
            b_IsDelayComplete = true;

            yield return null;
        }


        public void SetIsPowerUpAvailable(bool state)
        {
            b_PowerUpsAllowed = state;
        }
    }
}