using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    [CreateAssetMenu(fileName = "globalTextDatas", menuName = "TS/globalTextDatas")]
    public class globalTextDatas : ScriptableObject
    {
        public List<TextDatas> textDatasList = new List<TextDatas>();

        public string newDatasName = "New Datas";
        public TextDatas emptyTextDatas;

        public bool MoreOptions;
        public bool HelpBox;
        public int currentelectedDatas = 0;

        public List<bool> toggleLanguageList = new List<bool>();

        public bool b_UpdateAfterNewTextDataCreation;

        public int StartPos = 0;
        public int EndPos = 50;

        public int tab = 0;

        public List<string> newEntryTextList = new List<string>();
    }
}

