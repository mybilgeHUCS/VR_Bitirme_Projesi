// Description: VehicleFlagManager: Access any init the vehicle flags displayed on UI for P1 and P2
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace TS.Generics
{
    public class VehicleFlagManager : MonoBehaviour
    {
        public static VehicleFlagManager instance = null;

        public float speed = 50;

        public bool b_InitDone;
        private bool b_InitInProgress;

        public RectTransform flagPrefab;
        public VehicleUIColorsDatas vehicleUIColorsDatas;

        [System.Serializable]
        public class P1Paramters
        {
            public List<RectTransform> listPlaneFlags = new List<RectTransform>();
            public List<Vector3> listLastPos = new List<Vector3>();
        }

        //[HideInInspector]
        public List<P1Paramters> listPlayerFlagsInfo = new List<P1Paramters>();
        [HideInInspector]
        public List<Camera> listCams = new List<Camera>();

        public Vector3 flagScale = new Vector3(2,2,2);
        public Vector3 ReduceFlagSizeWithEnemyDistance = Vector3.one;

        public bool bFlagAllowed;

        public float flagAlpha = .7f;
        public float fadeSpeed = 1;

        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
        }

        void Start()
        {
            //cam = GetComponent<Camera>();
        }

        private void OnDestroy()
        {

        }

        //-> Init Lap counter
        public bool bInitVehicleFlag()
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(bInitRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;


            return b_InitDone;
            #endregion
        }

        IEnumerator bInitRoutine()
        {
            #region
            b_InitDone = false;

            int howManyRealPlayer = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer;

            for (var j = 0; j < howManyRealPlayer; j++)
            {
                listPlayerFlagsInfo.Add(new P1Paramters());
                listCams.Add(VehiclesVisibleByTheCamList.instance.listVehiclesVisibleByCamera[j].cam);
                for (var i = 0; i < VehiclesRef.instance.listVehicles.Count; i++)
                {
               
                    RectTransform newFlag = Instantiate(flagPrefab, CanvasInGameUIRef.instance.listPlayerUIElements[j].listRectTransform[1]);
                    newFlag.GetComponent<Image>().color = vehicleUIColorsDatas.listVehicleUIColorsDatas[i];
                    if (i < 10) newFlag.name = "Flag_0" + i;
                    else newFlag.name = "Flag_" + i;
                    newFlag.gameObject.SetActive(false);
                    newFlag.GetComponent<Image>().color = new Color(newFlag.GetComponent<Image>().color.r, newFlag.GetComponent<Image>().color.g, newFlag.GetComponent<Image>().color.b, 0);
                    listPlayerFlagsInfo[j].listPlaneFlags.Add(newFlag);
                    listPlayerFlagsInfo[j].listLastPos.Add(Vector3.zero);
                }
            }
            b_InitDone = true;
            yield return null;
            #endregion
        }

        

        public IEnumerator FlagColorFadeRoutine()
        {
            #region
            //Debug.Log("here");
            int howManyRealPlayer = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer;
            //int howManyVehicle = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.howManyVehicleInSelectedGameMode;
            int howManyVehicle = GameModeGlobal.instance.vehicleIDList.Count;

            float t = 0;
            yield return new WaitUntil(() => listPlayerFlagsInfo.Count != 0);
            yield return new WaitUntil(() => listPlayerFlagsInfo[0].listPlaneFlags.Count == howManyVehicle);
            while (t != flagAlpha)
             {
                if (!PauseManager.instance.Bool_IsGamePaused)
                 {
                     t += Time.deltaTime * fadeSpeed;
                     t = Mathf.Clamp(t, 0, flagAlpha);
                     for (var j = 0; j < howManyRealPlayer; j++)
                     {
                         for (var i = 0; i < listPlayerFlagsInfo[j].listPlaneFlags.Count; i++)
                         {
                             Color color = listPlayerFlagsInfo[j].listPlaneFlags[i].GetComponent<Image>().color;
                             listPlayerFlagsInfo[j].listPlaneFlags[i].GetComponent<Image>().color = new Color(color.r, color.g, color.b, t);
                         }
                     }
                 }
                 yield return null;
             }
            yield return null;
            #endregion
        }
    }

}
