using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TS.Generics
{
    public class VehiclePrefabPause : MonoBehaviour
    {
        public bool b_AutoInit;
        public bool b_InitDone;
        private bool b_InitInProgress;
        [HideInInspector]
        public bool SeeInspector;
        [HideInInspector]
        public bool moreOptions;
        [HideInInspector]
        public bool helpBox = true;

        public CallMethods_Pc callMethods;                              // Access script taht allow to call public function in this script.


        public List<EditorMethodsList_Pc.MethodsList> PauseMethodsList       // Create a list of Custom Methods that could be edit in the Inspector
                 = new List<EditorMethodsList_Pc.MethodsList>();
        public List<EditorMethodsList_Pc.MethodsList> UnPauseMethodsList       // Create a list of Custom Methods that could be edit in the Inspector
                 = new List<EditorMethodsList_Pc.MethodsList>();

        public int playerNumber = 0;

        public VehicleInfo vehicleInfo;

        private void Start()
        {
            if (PauseManager.instance)
            {
                PauseManager.instance.OnPause += Pause;
                PauseManager.instance.OnUnPause += UnPause;
            }
           
        }

        private void OnDestroy()
        {
            if (PauseManager.instance)
            {
                PauseManager.instance.OnPause -= Pause;
                PauseManager.instance.OnUnPause -= UnPause;
            }
        }

        public void Pause(int SelectedPause)
        {
            StartCoroutine(PauseRoutine(SelectedPause));
        }

        public void UnPause(int SelectedPause)
        {
            StartCoroutine(UnPauseRoutine(SelectedPause));
        }

        //-> Call all the methods in the list 
        IEnumerator PauseRoutine(int SelectedPause)
        {
            #region
            for (var i = 0; i < PauseMethodsList.Count; i++)
            {
                yield return new WaitUntil(() => callMethods.Call_One_Bool_Method(PauseMethodsList, i) == true);
            }
            yield return null;
            #endregion
        }

        IEnumerator UnPauseRoutine(int SelectedPause)
        {
            #region
            for (var i = 0; i < UnPauseMethodsList.Count; i++)
            {
                yield return new WaitUntil(() => callMethods.Call_One_Bool_Method(UnPauseMethodsList, i) == true);
            }
            yield return null;
            #endregion
        }
    }
}

