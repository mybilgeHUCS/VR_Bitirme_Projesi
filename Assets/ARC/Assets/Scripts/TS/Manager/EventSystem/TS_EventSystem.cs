//Description: TS_EventSystem: Easily access eventSystem and standaloneInputModule
using UnityEngine;
using UnityEngine.EventSystems;

namespace TS.Generics
{
    public class TS_EventSystem : MonoBehaviour
    {
        public static TS_EventSystem instance = null;
        public EventSystem eventSystem;
        public StandaloneInputModule standaloneInputModule;

        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;

            //-> If instance already exists and it's not this:
            else if (instance != this)
                Destroy(gameObject);
        }

        
    }
}
