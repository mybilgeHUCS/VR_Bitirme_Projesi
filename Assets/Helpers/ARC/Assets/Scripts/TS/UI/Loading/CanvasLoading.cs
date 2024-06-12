// Description: CanvasLoading: Scripts uses as a Tag
using UnityEngine;


namespace TS.Generics
{
    public class CanvasLoading : MonoBehaviour
    {
     
        public static CanvasLoading instance = null;


        void Awake()
        {
            #region Create ony one instance of the gameObject in the Hierarchy
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
            #endregion
        }
    }

}
