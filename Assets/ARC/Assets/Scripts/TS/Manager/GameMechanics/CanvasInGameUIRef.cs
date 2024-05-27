// Description: CanvasInGameUIRef: Access UI elements from any script
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace TS.Generics
{
    public class CanvasInGameUIRef : MonoBehaviour
    {
        public static CanvasInGameUIRef instance = null;

        [HideInInspector]
        public bool SeeInspector;
        [HideInInspector]
        public bool moreOptions;
        [HideInInspector]
        public bool helpBox = true;

        public int currentSelectedList;

        [System.Serializable]
        public class PlayerUIElements
        {
            public string name;
            public bool b_EditName;
            public Camera cam;
            public List<RectTransform> listRectTransform = new List<RectTransform>();
            public List<CurrentText> listTexts = new List<CurrentText>();
            public List<Image> listImage = new List<Image>();
        }

        public List<PlayerUIElements> listPlayerUIElements = new List<PlayerUIElements>();

        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
        }
    }

}
