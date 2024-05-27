//Description: TransitionManager.cs. The script manage transtions between two pages in the CanvasMainMenu.
// It is possible to create transition with script or by using an animation.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class TransitionManager : MonoBehaviour
    {
        public static TransitionManager instance = null;

        public bool                     SeeInspector;
        public bool                     helpBox = true;
        public bool                     moreOptions;
        public bool                     editorAllowsScriptedTransition;
        public int                      editorCurrentSelectedTransition = 0;
        public bool                     currentTab;
        public bool                     editName;
        public string                   editorNewTransitionName;

        public bool                     isTransitionOn = false;
        public bool                     isTransitionPart1Progress = false;
        public bool                     isTransitionPart2Progress = false;

        public GameObject               objCanvasTransition;

        int currentTransition;

        [System.Serializable]
        public class Multimethods
        {
            public string                                   _Name = "";
            public int                                      whichTransitionType = 0; // 0: Use script // 1: Use animation

            public List<EditorMethodsList_Pc.MethodsList>   methodsListPart1       // Create a list of Custom Methods that could be edit in the Inspector
            = new List<EditorMethodsList_Pc.MethodsList>();

            public List<EditorMethodsList_Pc.MethodsList>   methodsListPart2       // Create a list of Custom Methods that could be edit in the Inspector
           = new List<EditorMethodsList_Pc.MethodsList>();

            public Animator _animator;
            public bool     bPauseAnimatorUntilNewPageIsDisplayed = true;
        }

        public List<Multimethods>       listMultiMethods = new List<Multimethods>();

        public CallMethods_Pc           callMethods;                              // Access script taht allow to call public function in this script.

        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
        }

        void Start()
        {
            for(var i = 0;i< listMultiMethods.Count; i++)
            {
                if (listMultiMethods[i]._animator) listMultiMethods[i]._animator.gameObject.SetActive(false);
            }
        }



        public IEnumerator Transition(int selectedTransition,bool b_IsAnimation = false)
        {
            InfoPlayerTS.instance.b_IsAvailableToDoSomething = false;
            isTransitionOn = true;
            isTransitionPart1Progress = true;

            currentTransition = selectedTransition;

            objCanvasTransition.SetActive(true);
            yield return new WaitUntil(() => objCanvasTransition.activeSelf == true);

            //-> Transition using scripting
            if (!b_IsAnimation)
            {
                //--> First part of the transition
                callMethods.Call_A_Method_Only_Boolean(listMultiMethods[selectedTransition].methodsListPart1);
                yield return new WaitUntil(() => isTransitionPart1Progress == false);

                //-> Wait until the new page is displayed on screen
                yield return new WaitUntil(() => CanvasMainMenuManager.instance.b_IsSwitchPageInProgress == false);

                //--> Second part of the transition
                isTransitionPart2Progress = true;
                callMethods.Call_A_Method_Only_Boolean(listMultiMethods[selectedTransition].methodsListPart2);
                yield return new WaitUntil(() => isTransitionPart2Progress == false);
                isTransitionOn = false;

                objCanvasTransition.SetActive(false);
                yield return new WaitUntil(() => objCanvasTransition.activeSelf == false);

                //-> Transition is finished
                InfoPlayerTS.instance.b_IsAvailableToDoSomething = true;
            }
            //-> Transtion using Animation
            else
            {
                listMultiMethods[selectedTransition]._animator.gameObject.SetActive(true);
                yield return new WaitUntil(() => listMultiMethods[selectedTransition]._animator.gameObject.activeSelf == true);

                listMultiMethods[selectedTransition]._animator.Play("Base Layer.Transition");
            }

            Debug.Log("Transition Done");
            yield return null;
        }

        //-> This method is used with transition that use Animation.
        // The method is called when the current page is disabled and the new page is enable
        public void TransitionPart1Ended()
        {
            if(listMultiMethods[currentTransition].bPauseAnimatorUntilNewPageIsDisplayed)
                listMultiMethods[currentTransition]._animator.speed = 0;

            isTransitionPart1Progress = false;
            isTransitionPart2Progress = true;
        }

        //-> This method is used with transition that use Animation.
        // The method is called when the animation ended.
        public void TransitionPart2Ended()
        {
            StartCoroutine(ITransitionPart2Ended());
        }

        public IEnumerator ITransitionPart2Ended()
        {
            yield return new WaitForEndOfFrame();
            TransitionManager.instance.isTransitionPart2Progress = false;
            isTransitionOn = false;

            //-> Disable the object objCanvasTransition (optimize FPS)
            objCanvasTransition.SetActive(false);
            // Wait until objCanvasTransition is disabled in the Hierarchy 
            yield return new WaitUntil(() => objCanvasTransition.activeInHierarchy == false);

            listMultiMethods[currentTransition]._animator.gameObject.SetActive(false);
            yield return new WaitUntil(() => listMultiMethods[currentTransition]._animator.gameObject.activeSelf == false);

            //-> Transition is finished
            InfoPlayerTS.instance.b_IsAvailableToDoSomething = true;
            //Debug.Log("Transition Done");
            yield return null;
        }

        public void UnPauseAnimation()
        {
            listMultiMethods[currentTransition]._animator.speed = 1;
            //Debug.Log("Unpause animation");
        }
    }

}
