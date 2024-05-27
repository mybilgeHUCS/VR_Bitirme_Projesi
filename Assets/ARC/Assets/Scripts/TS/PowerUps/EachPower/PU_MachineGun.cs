// Description: PU_MachineGun: Called by PowerUpsSystemAssisatnt on vehicle.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TS.Generics
{
    [System.Serializable]
    public class PU_MachineGun
    {
        public AudioSource              aPlaneLocked;
        public AudioSource              aPlaneWarning;
        public Image                    imTarget;

        public PowerUpsAIDetectVehicle  DetectVehicleFront;
        private AudioSource             aSourceMachineGun;
        public AudioClip                aClipMachineGun;
        public float                    aVolumeMachineGun = 1;
        public AudioClip                aClipMachineGunHit;
        public float                    aVolumeMachineGunHit = 1;

        [HideInInspector]
        public int                      howManyBullet = 20;
        public int                      refHowManyBullet = 20;
        public int                      refHowManyBulletAI = 10;
        public float                    fireRate = .15f;

        public List<ParticleSystem>     listParticles = new List<ParticleSystem>();

        public UnityEvent               ShootMethods;

        public bool                     b_Is_MachineGun_Available = true;
        private bool                    b_StopTheRoutine;

        private bool                    b_IsInitDone;

        public List<RectTransform>      listUIBullet = new List<RectTransform>();

        public List<GameObject>         ignoreObjsList = new List<GameObject>();


        public void InitMachineGunPowerUp(bool b_AI,AudioSource aSourcePowerUps, PowerUpsSystem powerUpsSystem)
        {
            //Debug.Log("Init Machine Gun");
            // Init Power Up when the scene is loaded
            if (!b_IsInitDone)
            {
                if (imTarget && !b_AI) imTarget.gameObject.SetActive(false);

                howManyBullet = refHowManyBullet;
                if (b_AI) howManyBullet = refHowManyBulletAI;

                 aSourceMachineGun = aSourcePowerUps;

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

                b_IsInitDone = true;
            }
            // Init Power Up during the game
            else
            {
                if (!b_AI)
                {
                    imTarget.gameObject.SetActive(true);
                    foreach(RectTransform rect in listUIBullet)
                    {
                        rect.gameObject.SetActive(true);
                    }
                    listUIBullet[0].transform.parent.gameObject.SetActive(true);
                }

                howManyBullet = refHowManyBullet;
                if (b_AI) howManyBullet = refHowManyBulletAI;
            }
        }

        public void MachineGunTarget(List<GameObject> listObjToIgnore)
        {
            bool bIgnore = false;
            foreach(GameObject obj in listObjToIgnore)
            {
                if(obj == DetectVehicleFront.currentPlaneDetected)
                {
                    bIgnore = true;
                    break;
                }
            }

            if (DetectVehicleFront.b_VehicleEnemyDetected && imTarget.color != Color.red && !bIgnore)
            {
                imTarget.color = Color.red;
                if (aPlaneLocked && !aPlaneLocked.isPlaying)
                {
                    //Debug.Log("here: " + DetectVehicleFront.currentPlaneDetected.name);
                    aPlaneLocked.PlayOneShot(aPlaneLocked.clip);
                }
                   
            }
            else if (!DetectVehicleFront.b_VehicleEnemyDetected && imTarget.color != Color.white)
                imTarget.color = Color.white;
        }


        public IEnumerator MachineGunRoutine(bool b_AI, MonoBehaviour mono)
        {
            if (b_AI && DetectVehicleFront &&  DetectVehicleFront.b_VehicleEnemyDetected && b_Is_MachineGun_Available
                ||
                !b_AI && b_Is_MachineGun_Available)
            {
                b_StopTheRoutine = false;
                bool stopProcess = false;
                if (b_AI)
                {
                  
                    b_Is_MachineGun_Available = false;
                    if (DetectVehicleFront.b_VehicleEnemyDetected && b_AI)
                    {
                        PowerUpsSystem powerUpsSystem = DetectVehicleFront.currentPlaneDetected.transform.parent.GetComponent<PowerUpsSystem>();
                        VehicleDamage vehicleDamage = DetectVehicleFront.currentPlaneDetected.transform.parent.GetComponent<VehicleDamage>();

                        if (!powerUpsSystem.b_EnemyAttackAllowed || vehicleDamage.b_Invincibility)
                        {
                            stopProcess = true;
                        }

                        for (var i = 0; i < 1; i++)
                        {
                            if (DetectVehicleFront.currentPlaneDetected)
                            {
                                float t = 0;
                                if (vehicleDamage.b_Invincibility)
                                    t = 1;
                                else
                                    powerUpsSystem.PlayerLockedWarning(160, new Color(1, 0, 0, .55f));

                               
                                while (t < 1)
                                {
                                    if (!PauseManager.instance.Bool_IsGamePaused)
                                    {
                                        t += Time.deltaTime;

                                        if (vehicleDamage.b_Invincibility)
                                            t = 1;

                                    }
                                    yield return null;
                                }
                            }
                        }

                        if (!powerUpsSystem.b_EnemyAttackAllowed || vehicleDamage.b_Invincibility)
                        {
                            stopProcess = true;
                        }
                    }
                }

                if (b_AI && stopProcess)
                {
                    //Debug.Log("La");
                }
                else
                {
                    //Debug.Log("Ici: b_AI" + b_AI + " | stopProcess: " +   stopProcess + " | Name: " + mono.transform.parent.name);
                    int AICounter = 0;
                    while (howManyBullet != 0)
                    {
                        if (b_AI && !DetectVehicleFront.b_VehicleEnemyDetected
                            ||
                            !b_AI && b_StopTheRoutine
                            ||
                            b_AI && stopProcess
                            )
                        {
                            b_Is_MachineGun_Available = true;
                            break;
                        }

                        ShootMethods?.Invoke();

                        if (aSourceMachineGun &&
                            aClipMachineGun &&
                            aSourceMachineGun.gameObject.activeInHierarchy)
                            aSourceMachineGun.PlayOneShot(aClipMachineGun, aVolumeMachineGun);

                        howManyBullet--;
                        AICounter++;

                        if(!b_AI)
                            listUIBullet[howManyBullet].gameObject.SetActive(false);

                        if (DetectVehicleFront.b_VehicleEnemyDetected)
                        {
                            VehicleDamage vehicleDamage = DetectVehicleFront.currentPlaneDetected.transform.parent.GetComponent<VehicleDamage>();
                            vehicleDamage.LifeUpdate(-1,0, aClipMachineGunHit,aVolumeMachineGunHit);

                        }


                        b_Is_MachineGun_Available = false;


                        float t = 0;
                        while (t < fireRate)
                        {
                            if (!PauseManager.instance.Bool_IsGamePaused)
                            {
                                t += Time.deltaTime;
                            }
                            yield return null;
                        }

                        b_Is_MachineGun_Available = true;

                        if (AICounter == 4 && b_AI)
                        {
                            b_Is_MachineGun_Available = true;
                            break;
                        }
                    }

                    if (howManyBullet == 0)
                        ResetMachineGun(b_AI);
                }   
            }

            b_Is_MachineGun_Available = true;

            yield return null;
        }

        public void ResetMachineGun(bool b_AI)
        {
            b_StopTheRoutine = true;
            b_Is_MachineGun_Available = true;
            if(howManyBullet == 0 && !b_AI && imTarget)
            {
                imTarget.gameObject.SetActive(false);
                listUIBullet[0].transform.parent.gameObject.SetActive(false);
            }
               
        }

        public void DisableMachineGun(bool b_AI)
        {
            b_StopTheRoutine = true;
            b_Is_MachineGun_Available = true;
            if (!b_AI && imTarget)
            {
                imTarget.gameObject.SetActive(false);
                listUIBullet[0].transform.parent.gameObject.SetActive(false);
            }
                
            howManyBullet = 0;
        }
        
        public void PlayerPlayWarningSound()
        {
            if (aPlaneWarning && !aPlaneWarning.isPlaying && aPlaneWarning.gameObject.activeInHierarchy) aPlaneWarning.Play();
        }

    }
}