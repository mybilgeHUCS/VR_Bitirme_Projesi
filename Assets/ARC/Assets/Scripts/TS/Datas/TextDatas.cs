using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    [CreateAssetMenu(fileName = "TextDatas", menuName = "TS/TextDatas")]
    public class TextDatas : ScriptableObject
    {
        [System.Serializable]
        public class cTexts
        {
            public List<string> multiLanguage = new List<string>();
        }

        public List<cTexts> TextsList = new List<cTexts>();
    }
}

