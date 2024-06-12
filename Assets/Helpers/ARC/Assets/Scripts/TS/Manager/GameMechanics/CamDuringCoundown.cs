// Description: CamDuringCoundown: Allows to create Camera Sequence during Countdown. Call from the SceneStepManager bStep5CameraPreRace.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TS.Generics
{
    public class CamDuringCoundown : MonoBehaviour
    {
        [HideInInspector]
        public bool SeeInspector;
        [HideInInspector]
        public bool moreOptions;
        [HideInInspector]
        public bool helpBox = true;

        public int editorSelectedList;
        public string editorNewCountdownName;


        public static CamDuringCoundown instance = null;            // Static instance of GameManager which allows it to be accessed by any other script.
        public bool introAlreadyLoaded;                     // Use to know if it the first time the player open the Main Menu 

        public List<EditorMethodsList_Pc.MethodsList> methodsListLoad       // Create a list of Custom Methods that could be edit in the Inspector
        = new List<EditorMethodsList_Pc.MethodsList>();

        [System.Serializable]
        public class methodParams
        {
            public string _Name;
            public List<EditorMethodsList_Pc.MethodsList> methodsList       // Create a list of Custom Methods that could be edit in the Inspector
            = new List<EditorMethodsList_Pc.MethodsList>();
        }

        public List<methodParams> multiMethodsList = new List<methodParams>();


        [HideInInspector]
        public CallMethods_Pc callMethods;                              // Access script taht allow to call public function in this script.
  
        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;

            //-> If instance already exists and it's not this:
            else if (instance != this)
                Destroy(gameObject);
        }


        public bool BVehiclePresentationCountdown(int listID = 0)
        {
            StartCoroutine(VehiclePresentationCountdownRoutine(listID));
            return true;
        }

        IEnumerator VehiclePresentationCountdownRoutine(int listID = 0)
        {
            #region
            for (var i = 0; i < multiMethodsList[listID].methodsList.Count; i++)
            {
                yield return new WaitUntil(() => callMethods.Call_BoolMethod_CheckIfReturnTrue(multiMethodsList[listID].methodsList, i) == true);
            }
            yield return null;
            #endregion
        }
    }
}
