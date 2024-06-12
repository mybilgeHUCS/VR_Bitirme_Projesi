//Description: SwitchDesktopMobileManager.cs. A list of objects enabled or disabled depending the platform (desktop/Mobile).
// Use in association with w_SwitchPlatform.cs
using System.Collections.Generic;
using UnityEngine;
namespace TS.Generics
{
    public class ObjectsDependingPlatform : MonoBehaviour
    {
        public bool SeeInspector;
        public bool moreOptions;
        public bool helpBox = true;

        [System.Serializable]
        public class ObjectAvailable
        {
            public GameObject _Obj;
            public bool b_Desktop;
            public bool b_Mobile;
        }
        public List<ObjectAvailable> listDesktopMobileObjects = new List<ObjectAvailable>();
    }
}

