using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    [CreateAssetMenu(fileName = "TimeTrialModeData", menuName = "TS/TimeTrialModeData")]
    public class TimeTrialModeData : ScriptableObject
    {
        public bool         MoreOptions;
        public bool         HelpBox;

        public bool         bDisplayTrackUsingTrackListOrder = true;
        public List<int>    customTrackList = new List<int>();
        public bool         winCoinsAfterRace = true;

    }
}

