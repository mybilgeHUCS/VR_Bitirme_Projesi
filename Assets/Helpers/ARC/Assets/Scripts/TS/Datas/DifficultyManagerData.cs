using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    [CreateAssetMenu(fileName = "DifficultyManagerData", menuName = "TS/DifficultyManagerData")]
    public class DifficultyManagerData : ScriptableObject
    {
        public int howManyCarAI = 8;

        [System.Serializable]
        public class AICarParams
        {
            public float    speedOffset = 0;
            public float    aggressiveness = 1;
            public float    chooseBestAltPath = 100;
            public float    aiBooster = 100;


            public AICarParams(
                float _speedOffset,
                float _aggressiveness,
                float _chooseBestAltPath,
                float _aiBooster)
            {
                speedOffset = _speedOffset;
                aggressiveness = _aggressiveness;
                chooseBestAltPath = _chooseBestAltPath;
                aiBooster = _aiBooster;
               
            }
        }


        [System.Serializable]
        public class DiffifultyParams
        {
            public string name = "Difficulty Name";
            public List<AICarParams> aICarParams = new List<AICarParams>();
            public int folderTxt = 0;
            public int idTxt = 0;

            public DiffifultyParams(
                string _name,
                List<AICarParams> _aICarParams,
                int _folderTxt,
                int _idTxt)
            {
                name = _name;
                aICarParams = _aICarParams;
                folderTxt = _folderTxt;
                idTxt = _idTxt;
            }
        }

        public List<DiffifultyParams> difficultyParamsList = new List<DiffifultyParams>();

        public string   difficultyName = "New Difficulty";
        public bool     bEditDifficultyName;
        public int      currentEditorDifficulty = 0;
        public int      tabDifficulty = 0;
    }
}

