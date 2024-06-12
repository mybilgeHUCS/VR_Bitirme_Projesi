//-> Description: InfoPlayerTS : Allow any script to access player info. Use to know if the player is able to do actions.
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class InfoPlayerTS : MonoBehaviour
    {
        public static InfoPlayerTS  instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.

        [HideInInspector]
        public bool                 SeeInspector;
        [HideInInspector]
        public bool                 moreOptions;
        [HideInInspector]
        public bool                 helpBox = true;
        [HideInInspector]
        public string               NewCheckStateNameEditor;
        [HideInInspector]
        public int                  currentCheckStateDisplayed = 0;
        [HideInInspector]
        public bool                 editNameEditor = false;

        public bool                 b_IsSentenceInProcess = false;          // Use to check if a text displayed letter by letter is in process
        public bool                 b_ProcessToDisplayNewPageInProgress;// Use to know if to know if the process to display a new page is finished
        public bool                 b_IsPageCustomPartInProcess = true;     // Use to check if a custom part of Page In or Page Out sequence is in process
        public bool                 b_IsAvailableToDoSomething = true;      // Use to check if the player can do something on screen
        


        [System.Serializable]
        public class CheckState
        {
            public string   s_Name = "";
            public bool     b_Use_IsSentenceInProcess = true;
            public bool     b_IsSentenceInProcess = false;

            public bool     b_Use_IsPageInProcess = true;
            public bool     b_IsPageCustomPartInProcess = false;
            
            public bool     b_Use_IsAvailableToDoSomething = true;
            public bool     b_IsAvailableToDoSomething = true;

            public bool     b_Use_ProcessToDisplayNewPageInProgress = true;
            public bool     b_ProcessToDisplayNewPageInProgress = false;
        }

        [HideInInspector]
        public List<CheckState> listCheckStates = new List<CheckState>();

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


        //-> return the state of the list.
        //If all conditions true the method return true.
        //If one condition return false the method return false
        public bool returnCheckState(int whichCheckState)
        {
            if (listCheckStates[whichCheckState].b_Use_IsSentenceInProcess &&
                listCheckStates[whichCheckState].b_IsSentenceInProcess != b_IsSentenceInProcess) return false;

            if (listCheckStates[whichCheckState].b_Use_IsAvailableToDoSomething &&
                listCheckStates[whichCheckState].b_IsAvailableToDoSomething != b_IsAvailableToDoSomething) return false;

            if (listCheckStates[whichCheckState].b_Use_IsPageInProcess &&
                listCheckStates[whichCheckState].b_IsPageCustomPartInProcess != b_IsPageCustomPartInProcess) return false;

            if (listCheckStates[whichCheckState].b_Use_ProcessToDisplayNewPageInProgress &&
                listCheckStates[whichCheckState].b_ProcessToDisplayNewPageInProgress != b_ProcessToDisplayNewPageInProgress) return false;


            return true;
        }
    }

}
