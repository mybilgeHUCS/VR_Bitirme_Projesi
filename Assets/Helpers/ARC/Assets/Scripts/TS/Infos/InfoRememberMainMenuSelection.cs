//Description: InfoRememberMainMenuSelection. Access info about How many player | Current Game Mode | Current Difficulty
using UnityEngine;

namespace TS.Generics
{
    public class InfoRememberMainMenuSelection : MonoBehaviour
    {
        public static InfoRememberMainMenuSelection instance = null;            

        [HideInInspector]
        public bool                 SeeInspector;
        [HideInInspector]
        public bool                 moreOptions;
        [HideInInspector]
        public bool                 helpBox = true;

        public PlayerMainMenuSelection playerMainMenuSelection;

        void Awake()
        {
            #region Create ony one instance of the gameObject in the Hierarchy
            //Check if instance already exists
            if (instance == null)
                //if not, set instance to this
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
            #endregion
        }

    }

}
