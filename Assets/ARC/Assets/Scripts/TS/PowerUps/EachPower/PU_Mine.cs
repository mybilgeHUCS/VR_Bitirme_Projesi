// Description: PU_Mine: Called by PowerUpsSystemAssisatnt on vehicle.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    [System.Serializable]
    public class PU_Mine
    {
        public AudioClip                mineSound;
        public float                    aVolume = 1;
        public bool                     b_IsMineActivated;
        public bool                     b_IsMineAvailable;
        public int                      howManyMines = 2;
        public int                      howManyMinesAI = 1;
        [HideInInspector]
        public int                      currentMineNumber;

        public GameObject               minePrefab;
        public Transform                instantiatePosition;

        public float                    timeBetweenTwoMines = 1;

        public PowerUpsAIDetectVehicle  aiDetectVehicle;

        public List<RectTransform>      listUIMine = new List<RectTransform>();
        private bool                    b_IsInitDone;
        private bool                    b_AI;

        public Transform                objMineFeedback;

        public void InstantiateMinePrefab(AudioSource aSource,MonoBehaviour mono)
        {
            GameObject.Instantiate(minePrefab, instantiatePosition.position, instantiatePosition.rotation);

            if (!b_AI)
                mono.StartCoroutine(PlayerFeedbackRoutine(.2f));

            if (mineSound && aSource && aSource.gameObject.activeInHierarchy)
            {
                aSource.clip = mineSound;
                aSource.volume = aVolume;
                aSource.Play();
            }
            currentMineNumber--;
            if (!b_AI && listUIMine.Count > 0)
                listUIMine[currentMineNumber].gameObject.SetActive(false);
        }

        public void InitMinePowerUp(MonoBehaviour mono, bool b_RefAI, PowerUpsSystem powerUpsSystem)
        {
            
            b_IsMineActivated = true;
            b_IsMineAvailable = true;
            currentMineNumber = howManyMines;
            if (b_RefAI) currentMineNumber = howManyMinesAI;

            // Init Power Up when the scene is loaded
            if (!b_IsInitDone)
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
                            aiDetectVehicle = cam.EnemyDetector_02_Back;
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
                    if (currentMineNumber > 1)
                    {
                        foreach (RectTransform rect in listUIMine)
                        {
                            rect.gameObject.SetActive(true);
                        }
                        if (listUIMine.Count > 0)
                            listUIMine[0].transform.parent.gameObject.SetActive(true);
                    }
                }
            }
            //Debug.Log("Mine Init");
        }

       

        public void DisableMinePowerUp()
        {
            currentMineNumber = 0;
            b_IsMineActivated = false;

            if (!b_AI && listUIMine.Count > 0)
                listUIMine[0].transform.parent.gameObject.SetActive(false);
        }

        public void CheckAIEnabledMine(AudioSource aSource, bool b_IsPowerUpDetected, MonoBehaviour mono)
        {
            if (aiDetectVehicle &&
                (b_IsPowerUpDetected || aiDetectVehicle.b_VehicleEnemyDetected) &&
                b_IsMineAvailable &&
                b_IsMineActivated)
            {
                mono.StartCoroutine(AIWait(aSource, b_IsPowerUpDetected, mono));
            }
        }

        IEnumerator AIWaitBetweenTwoMine()
        {
            float t = 0;

            while(t != timeBetweenTwoMines)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
                    t = Mathf.MoveTowards(t, timeBetweenTwoMines, Time.deltaTime);
                    if (!b_IsMineActivated) break;
                }
                yield return null;
            }

            b_IsMineAvailable = true;
            yield return null;
        }

        IEnumerator AIWait(AudioSource aSource, bool b_IsPowerUpDetected, MonoBehaviour mono)
        {
            float t = 0;
            b_IsMineAvailable = false;
            mono.StartCoroutine(PlayerFeedbackRoutine(2));

            while (t != 2)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
                    t = Mathf.MoveTowards(t, 2, Time.deltaTime);
                    if (!b_IsMineActivated) break;
                }
                yield return null;
            }

            if (b_IsMineActivated)
            {
                b_IsMineActivated = true;
                b_IsMineAvailable = true;

                InstantiateMinePrefab(aSource,mono);
                mono.StartCoroutine(AIWaitBetweenTwoMine());
            }

            yield return null;
        }

        IEnumerator PlayerFeedbackRoutine(float duration)
        {
            float t = 0;
            objMineFeedback.gameObject.SetActive(true);
            while (t < 1)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
                    //t = Mathf.MoveTowards(t, 2, Time.deltaTime);

                    t += Time.deltaTime / duration;

                    objMineFeedback.localScale = Vector3.Lerp(Vector3.zero, new Vector3(1,1,1), t);

                    //if (!b_IsMineActivated) break;
                }
                yield return null;
            }
            objMineFeedback.localScale = Vector3.zero;
            objMineFeedback.gameObject.SetActive(false);

            yield return null;
        }
    }
}