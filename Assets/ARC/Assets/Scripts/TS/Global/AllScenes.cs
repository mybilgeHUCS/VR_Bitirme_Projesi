// Description: IntroInfo: Use to know if it the first time the player open the Main Menu 
using UnityEngine;


namespace TS.Generics
{
    public class AllScenes : MonoBehaviour
    {
        public static AllScenes instance = null;            // Static instance of GameManager which allows it to be accessed by any other script.

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
            DontDestroyOnLoad(gameObject);
        }
    }
}
