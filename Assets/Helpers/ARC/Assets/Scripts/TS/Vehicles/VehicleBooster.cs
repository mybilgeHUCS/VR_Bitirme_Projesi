using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TS.ARC;
using UnityEngine.Events;

namespace TS.Generics
{
    public class VehicleBooster : MonoBehaviour
    {
        public bool b_InitDone;
        private bool b_InitInProgress;
        private VehiclePrefabInit vehiclePrefabInit;

        [HideInInspector]
        public bool SeeInspector;
        [HideInInspector]
        public bool moreOptions;
        [HideInInspector]
        public bool helpBox = true;

        public List<EditorMethodsList_Pc.MethodsList> methodsList       // Create a list of Custom Methods that could be edit in the Inspector
        = new List<EditorMethodsList_Pc.MethodsList>();

        public CallMethods_Pc callMethods;                              // Access script taht allow to call public function in this script.

        private VehicleAI vehicleAI;
        private VehicleInfo vehicleInfo;
        private VehicleInputs vehicleInputs;
        private VehicleDamage vehicleDamage;

        [Header("Booster")]
        
        public float HowManyBoost = 100;                            // Add more speed to the plane is player press the booster input
        private float HowManyBoostRef = 100;
        public float speedToReachMaxBooster = 150;
        public float speedToReachMinBooster = 100;
        public float boosterDurationSpeed = 2;
        private float boosterDurationSpeedRef = 2;
        public float boosterCoolDownSpeed = 1;
        public bool b_Booster;
        //[HideInInspector]
        public float currentBoost;
        public AudioSource aSourceBoosterSound;
        public AudioClip sfxOn;
        public AudioClip sfxOff;
        public Image imBoosterBar;
        private RectTransform rectBoosterBar;
        private float   refWidthBoosterBar;
        //public GameObject objParticleSpeedFx;
        public float BoostGauge = 1;

        public UnityEvent BoosterOn;
        public UnityEvent BoosterOff;

        void Start()
        {
            #region
            vehicleAI = GetComponent<VehicleAI>();
            vehicleInfo = GetComponent<VehicleInfo>();
            vehicleInputs = GetComponent<VehicleInputs>();
            vehicleInputs.BoosterGetKeyPressed += BoosterGetKeyPressed;
            vehicleInputs.BoosterGetKeyReleased += BoosterGetKeyReleased;

            vehicleDamage = GetComponent<VehicleDamage>();
            vehicleDamage.VehicleExplosionAction += VehicleExplosion;
            vehicleDamage.VehicleRespawnPart2 += VehicleRespawnPart2;
            #endregion
        }

        public void OnDestroy()
        {
            #region
            vehicleInputs.BoosterGetKeyPressed      -= BoosterGetKeyPressed;
            vehicleInputs.BoosterGetKeyReleased     -= BoosterGetKeyReleased;

            vehicleDamage.VehicleExplosionAction    -= VehicleExplosion;
            vehicleDamage.VehicleRespawnPart2       -= VehicleRespawnPart2;
            #endregion
        }

        //-> Initialisation
        public bool bInitVehicleBooster()
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
            vehiclePrefabInit = transform.parent.GetComponent<VehiclePrefabInit>();

            //-> Connect the UI Booster Gauge
            if (vehicleInfo.playerNumber == 0 || vehicleInfo.playerNumber == 1)
            {
                GameObject canvasInGame = GameObject.FindGameObjectWithTag("CanvasInGame");
                if (canvasInGame)
                {
                    BoosterGaugeTag[] boosterGauge = canvasInGame.GetComponentsInChildren<BoosterGaugeTag>(true);

                    foreach (BoosterGaugeTag obj in boosterGauge)
                    {
                        if (obj && obj.PlayerID == vehicleInfo.playerNumber)
                        {
                            imBoosterBar = obj.GetComponent<Image>();
                            rectBoosterBar = obj.GetComponent<RectTransform>();
                            refWidthBoosterBar = rectBoosterBar.rect.width;
                        }
                    }
                }
            }

            if (DataRef.instance.vehicleGlobalData)
            {
                int vehicleDataID = vehiclePrefabInit.vehicleDataID;

                //-> Init vehicle booster using vehicle data
                // P1 | P2
                if (vehicleInfo.playerNumber == 0
                    ||
                    vehicleInfo.playerNumber == 1 && InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer == 2)
                {
                    HowManyBoost = DataRef.instance.vehicleGlobalData.carParametersList[vehicleDataID].boosterPower;
                    boosterDurationSpeed = Mathf.Clamp(DataRef.instance.vehicleGlobalData.carParametersList[vehicleDataID].boosterDuration, .1f, 1000);
                    boosterCoolDownSpeed = Mathf.Clamp(DataRef.instance.vehicleGlobalData.carParametersList[vehicleDataID].boosterCooldown,.1f,1000);
                }
                else
                {
                    HowManyBoost = DataRef.instance.vehicleGlobalData.carParametersList[vehicleDataID].boosterPowerAI;
                    boosterDurationSpeed = Mathf.Clamp(DataRef.instance.vehicleGlobalData.carParametersList[vehicleDataID].boosterDurationAI, .1f, 1000);
                    boosterCoolDownSpeed = Mathf.Clamp(DataRef.instance.vehicleGlobalData.carParametersList[vehicleDataID].boosterDurationAI, .1f, 1000);
                }

                HowManyBoostRef = HowManyBoost;
                boosterDurationSpeedRef = boosterDurationSpeed;
            }

            b_InitDone = true;
           // Debug.Log("Init: VehicleBooster -> Done");
            yield return null;
        }

        private void Update()
        {
            #region
            if (b_InitDone &&
                vehiclePrefabInit &&
                vehiclePrefabInit.b_InitDone &&
                !PauseManager.instance.Bool_IsGamePaused)
                CoolDownTimer();

            
            if (b_InitDone &&
                vehiclePrefabInit &&
                vehiclePrefabInit.b_InitDone &&
                !PauseManager.instance.Bool_IsGamePaused &&
                (!vehicleAI.enabled &&
                vehicleInfo.b_IsPlayerInputAvailable
                ||
                vehicleAI.enabled)
                )
            {
                //-> Check Booster
                Booster();
            }
            #endregion
        }


        void Booster()
        {
            #region
            if (vehicleInfo.b_IsVehicleAvailableToMove  &&
                b_Booster &&
                !vehicleInfo.b_IsRespawn)
            {
                currentBoost = Mathf.MoveTowards(currentBoost, HowManyBoost, Time.deltaTime * speedToReachMaxBooster);
                BoosterOn.Invoke();
            }
            else
            {
                currentBoost = Mathf.MoveTowards(currentBoost, 0, Time.deltaTime * speedToReachMinBooster);
                BoosterOff.Invoke();
            }
            #endregion
        }



        IEnumerator BoosterRoutine(bool b_PowerUpCase = false)
        {
            b_Booster = true;

            if (!vehicleAI.enabled && aSourceBoosterSound.gameObject.activeInHierarchy)
            {
                aSourceBoosterSound.clip = sfxOn;
                aSourceBoosterSound.Play();
            }

            // Regular Booster
            if (!b_PowerUpCase)
            {
                float tmp = BoostGauge;
                WaitBeforeReloadBooster = 0;


                while (tmp != 0.0f)
                {
                    if (!PauseManager.instance.Bool_IsGamePaused)
                    {
                        tmp = Mathf.MoveTowards(tmp, 0.0f, Time.deltaTime * 1 / boosterDurationSpeed);
                        BoostGauge = tmp;

                        if (!vehicleAI.enabled && rectBoosterBar)
                            rectBoosterBar.sizeDelta = new Vector2(Mathf.MoveTowards(BoostGauge * refWidthBoosterBar , 1, 1), rectBoosterBar.rect.height) ;

                        if (!b_Booster)
                        {tmp = 0.0f;}  
                    }

                    yield return null;
                }

                if (!vehicleAI.enabled && aSourceBoosterSound.gameObject.activeInHierarchy)
                {
                    aSourceBoosterSound.clip = sfxOff;
                    aSourceBoosterSound.Play();
                }

                b_Booster = false;
            }
            //-> Power-Up case
            else if (b_PowerUpCase)
            {
                // Ring Booster:
                // Stuck booster gauge. Add booster to vehicle. When boost ended allows regular boost again.
            }

            yield return null;
        }


        public bool B_AIEnableBooster()
        {
            if (!b_Booster && BoostGauge == 1 &&
                vehicleAI.enabled)
            {
                //Debug.Log("AI Booster");
                StartCoroutine(BoosterRoutine(false));
            }
            return true;
        }

        public bool B_EnableBoosterPowerUp()
        {
            /*if (!b_Booster && BoostGauge == 1 && vehicleAI.enabled)
            {
                StopAllCoroutines();
                StartCoroutine(BoosterRoutine(true));
            }
           */
            return true;
        }

        public void BoosterGetKeyPressed()
        {
            if (!b_Booster &&
                BoostGauge > 0 &&
                vehicleInfo.b_IsPlayerInputAvailable &&
                !PauseManager.instance.Bool_IsGamePaused)
            {
                b_ButtonUp = false;
                StartCoroutine(BoosterRoutine(false));
            }   
        }

        private bool b_ButtonUp = false;
        public void BoosterGetKeyReleased()
        {
            if (!PauseManager.instance.Bool_IsGamePaused)
            {
                b_Booster = false;
                b_ButtonUp = true;
            }
        }

        private float WaitBeforeReloadBooster = 0;
        //private float GaugeSize;
        public void CoolDownTimer()
        {
            if (!vehicleAI.enabled && b_ButtonUp ||
                vehicleAI.enabled)
            {
                if (!vehicleInfo.b_IsRespawn)
                {
                    //-> P1 P2: The booster for 2s if the gauge = 0
                    if (!b_Booster && WaitBeforeReloadBooster != 2 && !vehicleAI.enabled)
                    {
                        WaitBeforeReloadBooster = Mathf.MoveTowards(WaitBeforeReloadBooster, 2, Time.deltaTime);
                    }
                    //-> AI: There is no waiting time between the moment when the gauge is 0 and the moment when the AI can use it again
                    else if (vehicleAI.enabled && WaitBeforeReloadBooster != 2)
                    {
                        WaitBeforeReloadBooster = 2;
                    }

                    //-> Cooldown (AI and P1 | P2)
                    if (WaitBeforeReloadBooster == 2)
                    {
                        BoostGauge = Mathf.MoveTowards(BoostGauge, 1, Time.deltaTime * 1 / boosterCoolDownSpeed);
                        //if(imBoosterBar)imBoosterBar.transform.localScale = new Vector3(BoostGauge, 1, 1);

                        if (rectBoosterBar)
                            rectBoosterBar.sizeDelta = new Vector2(Mathf.MoveTowards(BoostGauge * refWidthBoosterBar, 1, 1), rectBoosterBar.rect.height);
                    }
                }
            }
        }


        public int counter = 0;
        public bool bLastTriggerIsBehind;
        float difficultyRatio = 0;  
        void OnTriggerEnter(Collider other)
        {
            if (b_InitDone && vehiclePrefabInit && vehiclePrefabInit.b_InitDone)
            {
                //-> Check 
                if (other.GetComponent<TriggerAIBooster>() && vehicleAI.enabled)
                {
                    //-> Override booster values
                    TriggerAIBooster triggerAIBooster = other.GetComponent<TriggerAIBooster>();

                    int currentAIDifficulty = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentDifficulty;



                    //-> Booster offset
                    if (IsAIInFrontOfPlayers())
                    {
                        if (bLastTriggerIsBehind)
                            counter = 0;

                        difficultyRatio = (DataRef.instance.difficultyManagerData.difficultyParamsList[currentAIDifficulty].aICarParams[vehicleInfo.playerNumber].aiBooster + 10f * counter) * .01f;

                        if (counter > -10 && !bLastTriggerIsBehind)
                            counter--;



                        bLastTriggerIsBehind = false;
                    }
                    else
                    {
                        if (!bLastTriggerIsBehind)
                            counter = 0;

                        if (counter < 4 && bLastTriggerIsBehind)
                            counter++;


                        float increase = 0;
                        if (currentAIDifficulty >=2)
                            increase = 7.5f;
                        else if (currentAIDifficulty == 1)
                            increase = 3f;
                        else if (currentAIDifficulty == 0)
                            increase = 2f;

                        difficultyRatio = (DataRef.instance.difficultyManagerData.difficultyParamsList[currentAIDifficulty].aICarParams[vehicleInfo.playerNumber].aiBooster + increase * counter) * .01f;

                        bLastTriggerIsBehind = true;
                    }


                    //Debug.Log("Ratio: " + difficultyRatio);

                    HowManyBoost = HowManyBoostRef * triggerAIBooster.boostInPercentage * .01f * difficultyRatio;
                    boosterDurationSpeed = boosterDurationSpeedRef * triggerAIBooster.durationInPercentage * .01f;


                    for (var i = 0; i < methodsList.Count; i++)
                    {
                        callMethods.Call_BoolMethod_CheckIfReturnTrue(methodsList, i);
                    }
                }
            }
        }

        public void PostFxChromaticAberationOn()
        {
            if (!vehicleAI.enabled)
            CamRef.instance.listPostFxVolumeProfile[vehicleInfo.playerNumber].BoosterFxOn();
        }

        public void PostFxChromaticAberationOff()
        {
            if (!vehicleAI.enabled)
                CamRef.instance.listPostFxVolumeProfile[vehicleInfo.playerNumber].BoosterFxOff();
        }

        public void PostFxColorAdjustmentOn()
        {
            if (!vehicleAI.enabled)
                CamRef.instance.listPostFxVolumeProfile[vehicleInfo.playerNumber].ColorAdjustmentsOn();
        }

        public void PostFxColorAdjustmentOff()
        {
            if (!vehicleAI.enabled)
                CamRef.instance.listPostFxVolumeProfile[vehicleInfo.playerNumber].ColorAdjustmentsOff();
        }

        public void VehicleExplosion()
        {
            StopAllCoroutines();
            if (!vehicleAI.enabled)
            {
                BoostGauge = 0;

                if (rectBoosterBar)
                    rectBoosterBar.sizeDelta = new Vector2(Mathf.MoveTowards(BoostGauge * refWidthBoosterBar, 1, 1), rectBoosterBar.rect.height);
            }
            currentBoost = Mathf.MoveTowards(currentBoost, 0, Time.deltaTime * 100);
            aSourceBoosterSound.Stop();

            HowManyBoost = HowManyBoostRef;
            boosterDurationSpeed = boosterDurationSpeedRef;

            BoosterGetKeyReleased();
        }

        public void BoosterSkakeCamOn()
        {
            if (!vehicleAI.enabled)
                CamRef.instance.listCameras[vehicleInfo.playerNumber].transform.parent.GetComponent<BoostShakeCam>().ShakeStart();
        }

        public void BoosterSkakeCamOff()
        {
            if (!vehicleAI.enabled)
                CamRef.instance.listCameras[vehicleInfo.playerNumber].transform.parent.GetComponent<BoostShakeCam>().ShakeStop();
        }

        public void VehicleRespawnPart2()
        {
            if (!vehicleAI.enabled)
            {
                BoostGauge = 1;

                if (rectBoosterBar)
                    rectBoosterBar.sizeDelta = new Vector2(Mathf.MoveTowards(BoostGauge * refWidthBoosterBar, 1, 1), rectBoosterBar.rect.height);
            }
        }


        //-> Check if AI is In front of Player 1 or Player 2
        bool IsAIInFrontOfPlayers()
        {
            if(vehicleInfo.playerNumber != 0 && LapCounterAndPosition.instance.posList.Count >= 2)
            {
                int AIPositionOnRace = LapCounterAndPosition.instance.posList[vehicleInfo.playerNumber].RacePos;

                int P1PosOnRace = LapCounterAndPosition.instance.posList[0].RacePos;

                if (AIPositionOnRace < P1PosOnRace)
                    return true;

                int howManyPlayer = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer;

                if (howManyPlayer > 1)
                {
                    int P2PosOnRace = LapCounterAndPosition.instance.posList[2].RacePos;
                    if (AIPositionOnRace < P2PosOnRace)
                        return true;
                }
            }
            
           
            return false;
        }
    }

}
