// Decription: CanvasAutoSave: Script use as a tag to access CanvasAutoSave object
using UnityEngine;

namespace TS.Generics
{
    public class CanvasAutoSave : MonoBehaviour
    {
        public static CanvasAutoSave instance = null;

        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
        }
    }

}
