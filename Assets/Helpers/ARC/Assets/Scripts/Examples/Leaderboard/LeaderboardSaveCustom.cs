// Description: LeaderboardSaveCustom: Script use as an example in the Documentation Part Section Leaderboard
// How to: Customize leaderboard save process
using UnityEngine;

namespace TS.Generics
{
    public class LeaderboardSaveCustom : MonoBehaviour
    {
        public void CustomSave()
        {
            //-> Player Name
            string playerName = LeaderboardSaveName.instance.sCurrentName;

            //-> Raw time:
            float rawTime = LapCounterAndPosition.instance.posList[0].globalTime;

            //-> Time as an int:  timeScore = (int)(LapCounterAndPosition.instance.posList[0].globalTime * 1000.0f);
            int intTime = LeaderboardSaveName.instance.timeScore;

            //-> Time formatted (minute, seconds, milliseconds)
            string formattedTime = LeaderboardSaveName.instance.FormatTimer(LeaderboardSaveName.instance.timeScore);


            // DoSomething with those values. (For example save the value in an online Leaderboard)
        }
    }
}

