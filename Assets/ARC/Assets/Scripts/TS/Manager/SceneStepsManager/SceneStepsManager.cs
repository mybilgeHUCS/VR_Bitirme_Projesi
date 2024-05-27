// Description: SceneStepsManager: Managed Game Mode steps
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class SceneStepsManager : MonoBehaviour
    {
        public static SceneStepsManager instance = null;
        [HideInInspector]
        public bool                     SeeInspector;
        [HideInInspector]
        public bool                     moreOptions;
        [HideInInspector]
        public bool                     helpBox = true;

        public int                      currentStepSequence = 0;
        public int                      currentStep = 0;

        public bool                     b_IsSceneStepManagerRunning;
        public int                      currentStepListDisplayedEditor = 0;


        [System.Serializable]
        public class C_MethodLists
        {
            public List<EditorMethodsList_Pc.MethodsList> methodsList       // Create a list of Custom Methods that could be edit in the Inspector
                = new List<EditorMethodsList_Pc.MethodsList>();
        }

        public List<C_MethodLists> multiMethodList = new List<C_MethodLists>();


        [System.Serializable]
        public class C_SceneStepsList
        {
            public string sName = "";
            public List<string> b_StepName = new List<string>();
            public List<bool> b_ShowBypass = new List<bool>();
            public List<bool> b_Bypass = new List<bool>();
            public List<bool> b_ShowAutoStart = new List<bool>();
            public List<bool> B_AutoStart = new List<bool>();
            public List<C_MethodLists> SceneStepsMultiList = new List<C_MethodLists>();
        }

        public List<C_SceneStepsList> SceneStepsList = new List<C_SceneStepsList>();


        public CallMethods_Pc callMethods;                              // Access script taht allow to call public function in this script.


        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
        }

        public bool NextStep(int StepSequence = -100,int StepID = -100)
        {
            currentStepListDisplayedEditor = 0;
            if (StepSequence != -100) currentStepSequence = StepSequence;
            if (StepID != -100) currentStep = StepID;
            StartCoroutine(NextStepCallAllTheMethodsOneByOne());
            return true;
        }

        //-> Call all the methods in the list 
        IEnumerator NextStepCallAllTheMethodsOneByOne()
        {
            #region
            b_IsSceneStepManagerRunning = true;
            //Debug.Log("1: currentStepSequence: " + currentStepSequence + ": currentStep: " + currentStep + " : SceneStepsList[currentStepSequence].SceneStepsMultiList.Count: " + SceneStepsList[currentStepSequence].SceneStepsMultiList.Count);
            if (currentStep < SceneStepsList[currentStepSequence].SceneStepsMultiList.Count)
            {
                for (var i = 0; i < SceneStepsList[currentStepSequence].SceneStepsMultiList[currentStep].methodsList.Count; i++)
                {
                    if (!SceneStepsList[currentStepSequence].b_Bypass[currentStep])
                        yield return new WaitUntil(() => callMethods.Call_One_Bool_Method(SceneStepsList[currentStepSequence].SceneStepsMultiList[currentStep].methodsList, i) == true);
                }

                currentStep++;

                //-> The next step is bypassed
                if (SceneStepsList[currentStepSequence].b_Bypass[currentStep - 1] &&
                    currentStep < SceneStepsList[currentStepSequence].SceneStepsMultiList.Count)
                    NextStep();

                //-> The next step is linked
                if (SceneStepsList[currentStepSequence].b_Bypass.Count > currentStep &&
                    !SceneStepsList[currentStepSequence].b_Bypass[currentStep] &&
                    SceneStepsList[currentStepSequence].B_AutoStart[currentStep] &&
                    currentStep < SceneStepsList[currentStepSequence].SceneStepsMultiList.Count)
                {
                    //Debug.Log("Next Step (Link)");
                    NextStep();
                }
            }
            else
            {
                //Debug.Log("No More Step");
            }

            b_IsSceneStepManagerRunning = false;
            yield return null;
            #endregion
        }


       


    }
}
