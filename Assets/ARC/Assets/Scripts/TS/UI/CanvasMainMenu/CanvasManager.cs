// Description: CanvasManager
using System.Collections.Generic;
using UnityEngine;


namespace TS.Generics
{
    public class CanvasManager : MonoBehaviour
    {
        public List<GameObject> listMenu = new List<GameObject>();
        public bool             SeeInspector;
        public bool             moreOptions;
        public int              toolbarInt = 0;
        public bool             helpBox = true;
        public int              currentSelectedPage = 0;
        public GameObject       RefNewPage;
    }
}

