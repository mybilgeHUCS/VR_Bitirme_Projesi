// Description: CheckEndOfTheRace: When a vehicle finish the race check if the game must display the result menu
// In Hierarchy -> GAMEPLAY -> MANAGERS ->
using System.Collections;
using UnityEngine;

namespace TS.Generics
{
    public class CheckEndOfTheRace : MonoBehaviour
    {
        public static CheckEndOfTheRace instance = null;
        public bool                     b_InitDone;
        private bool                    b_InitInProgress;
        bool                            bEndRaceOnce = false;

        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
        }

        private void Start()
        {
            LapCounterAndPosition.instance.AVechicleFinishTheRace += CheckEndOfTheTheRace;
        }

        private void OnDestroy()
        {
            LapCounterAndPosition.instance.AVechicleFinishTheRace -= CheckEndOfTheTheRace;
        }

        //-> Init Lap counter
        public bool bInitLapCounter()
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(bInitRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;

            return b_InitDone;
            #endregion
        }

        IEnumerator bInitRoutine()
        {
            #region
            b_InitDone = false;

           
            b_InitDone = true;
            yield return null;
            #endregion
        }

        //-> When a vehicle finish the race check if the game must display the result menu (SceneStepsManager.instance.NextStep())     
        void CheckEndOfTheTheRace(int playerID)
        {
            Debug.Log("The Race is Complete");
            int howManyPlayer = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer;
            bool bIsRaceComplete = true;
            LapCounterAndPosition lapCounterAndPosition = LapCounterAndPosition.instance;

            for (var i = 0;i< howManyPlayer; i++)
            {
                if (!lapCounterAndPosition.posList[i].IsRaceComplete)
                {
                    bIsRaceComplete = false;
                    break;
                }
            }

            Debug.Log("The Race is Complete: howManyPlayer ->" + howManyPlayer + " | bIsRaceComplete -> " + bIsRaceComplete);

            if (bIsRaceComplete && !bEndRaceOnce)
            {
                bEndRaceOnce = true;
                SceneStepsManager.instance.NextStep();
            }
        }
    }
}
