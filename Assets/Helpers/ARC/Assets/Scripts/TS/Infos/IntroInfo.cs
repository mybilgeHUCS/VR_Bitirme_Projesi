// Description: IntroInfo: Use to know if it the first time the player open the Main Menu
// + Load Data when the game starts
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TS.Generics
{
    public class IntroInfo : MonoBehaviour
    {
        [HideInInspector]
        public bool SeeInspector;
        [HideInInspector]
        public bool moreOptions;
        [HideInInspector]
        public bool helpBox = true;


        public static IntroInfo instance = null;            // Static instance of GameManager which allows it to be accessed by any other script.
        public bool introAlreadyLoaded;                     // Use to know if it the first time the player open the Main Menu 

        public List<EditorMethodsList_Pc.MethodsList> methodsListLoad       // Create a list of Custom Methods that could be edit in the Inspector
        = new List<EditorMethodsList_Pc.MethodsList>();

        public CallMethods_Pc callMethods;                              // Access script taht allow to call public function in this script.
        private bool isDataLoaded;

        public bool b_DisplayLoadingScreen = true;

        public GlobalDatas globalDatas;

        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;

            //-> If instance already exists and it's not this:
            else if (instance != this)
                Destroy(gameObject);
        }

        void Start()
        {
            //-> Init the scene if it is the first scene oopened by the player
            if (!introAlreadyLoaded)
            {
                StartCoroutine(InitGame());
            }   
        }

        IEnumerator InitGame()
        {
            if (CanvasLoadingManager.instance &&
                !CanvasLoadingManager.instance.objCanvasLoading.transform.GetChild(0).gameObject.activeSelf)
            {
               if(b_DisplayLoadingScreen) StartCoroutine(CanvasLoadingManager.instance.DisplayCanvasLoading());
            }
 
            //-> Load datas when the game is launched
            isDataLoaded = false;
            StartCoroutine(LoadDatasWhenGameIsLaunched());

            //-> Wait until the init is finised
            yield return new WaitUntil(() => isDataLoaded == true);

            //-> Call SceneINitManager to initialize the scene
            StartCoroutine(SceneInitManager.instance.CallAllTheMethodsOneByOne());

            introAlreadyLoaded = true;
            //-> Wait until the init is finished (SceneInitManager)
            yield return new WaitUntil(() => SceneInitManager.instance.b_IsInitDone == true);

            //-> Close Canvas Loading
            if (CanvasLoadingManager.instance)StartCoroutine(CanvasLoadingManager.instance.CloseCanvasLoading());
            yield return null;
        }

        IEnumerator LoadDatasWhenGameIsLaunched()
        {
            #region
            for (var i = 0; i < methodsListLoad.Count; i++)
            {
                yield return new WaitUntil(() => callMethods.Call_BoolMethod_CheckIfReturnTrue(methodsListLoad, i) == true);
            }

            isDataLoaded = true;
            //Debug.Log("Datas Launched done");
            yield return null;
            #endregion
        }
    }
}
