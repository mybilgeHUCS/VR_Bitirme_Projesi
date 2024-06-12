// Description: LoadSavePlayerProgession: Allow to load|Save player progression
// It is possible to access the script from anywhere
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TS.Generics;

namespace TS.Generics
{
    public class LoadSavePlayerProgession : MonoBehaviour
    {
        public static LoadSavePlayerProgession instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.

        [HideInInspector]
        public bool SeeInspector;
        [HideInInspector]
        public bool moreOptions;
        [HideInInspector]
        public bool helpBox = true;

        public int currentSelectedSlot = 0;

        public List<EditorMethodsList_Pc.MethodsList> methodsListSave       // Create a list of Custom Methods that could be edit in the Inspector
        = new List<EditorMethodsList_Pc.MethodsList>();

        public List<EditorMethodsList_Pc.MethodsList> methodsListLoad       // Create a list of Custom Methods that could be edit in the Inspector
       = new List<EditorMethodsList_Pc.MethodsList>();

        public CallMethods_Pc callMethods;                              // Access script taht allow to call public function in this script.

        public string SaveName = "PP_";

        //private bool b_IsLoadDone = false;

        public int loadInProgress;
        //public bool b_LoadIsFinished = false;

        void Awake()
        {
            #region Create ony one instance of the gameObject in the Hierarchy
            //Check if instance already exists
            if (instance == null)
                //if not, set instance to this
                instance = this;

            //If instance already exists and it's not this:
            else if (instance != this)
                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);
            #endregion
        }


        private void Update()
        {
            
        }

        //-> Create a string that contains all the information on the player's progress
        public string SaveInfoPlayer()
        {
            #region
            string sData = "";

            for (var i = 0; i < methodsListSave.Count; i++)
            {
                sData += callMethods.Call_One_String_Method(methodsListSave, i);
                sData += "|";
            }
            return sData;
            #endregion
        }

        //-> Call in SceneInitManager when the scene is load. It loads all the information on the player's progress
        public bool B_LoadPlayerProgression()
        {
            #region
            //-> Play the coroutine Once
            if (loadInProgress == 0)
            {
                loadInProgress = 1;
                StartCoroutine(LoadInfoPlayer());
            }
            //-> Check if the coroutine is finished
            else if (loadInProgress == 2)
                loadInProgress = 3;

            if (loadInProgress == 3)
            {
                loadInProgress = 0;
                return true;
            }
            else
                return false;
            #endregion
        }


        //-> Call all the methods in the list 
        IEnumerator LoadInfoPlayer()
        {
            #region
            string sData = SaveManager.instance.LoadDAT(SaveName + currentSelectedSlot);
            string[] codes = sData.Split('|');

            //Debug.Log("codes: " + codes.Length + "-> " + sData);

            if(sData != "")
                yield return new WaitUntil(() => callMethods.Call_A_Method_LoadInfoPlayer(methodsListLoad, codes) == true);
            else
            {
                List<string> tmpList = new List<string>();
                for (var i = 0; i < methodsListLoad.Count; i++) tmpList.Add("");
                 codes = new string[methodsListLoad.Count];
                codes = tmpList.ToArray();
                yield return new WaitUntil(() => callMethods.Call_A_Method_LoadInfoPlayer(methodsListLoad, codes) == true);
            }


            loadInProgress = 2;
            //Debug.Log("Player Progression Loaded part 1");
            //yield return new WaitForSeconds(1);
            //Debug.Log("Player Progression Loaded part 2");
            yield return null;
            #endregion
        }

        public void SavePlayerProgression()
        {
            StartCoroutine(SavePlayerProgressionRoutine());
        }

        public IEnumerator SavePlayerProgressionRoutine()
        {
            #region
            //-> Save New Player Progression
            InfoPlayerTS.instance.b_IsAvailableToDoSomething = false;
            CanvasAutoSave.instance.transform.GetChild(0).gameObject.SetActive(true);

            yield return new WaitUntil(() => SaveManager.instance.saveAndReturnTrueAFterSaveProcess(
                SaveInfoPlayer(),
                "PP_" + currentSelectedSlot));

            yield return new WaitForSeconds(.5f);
            CanvasAutoSave.instance.transform.GetChild(0).gameObject.SetActive(false);

            InfoPlayerTS.instance.b_IsAvailableToDoSomething = true;
            yield return null;
            #endregion
        }

    }

}
