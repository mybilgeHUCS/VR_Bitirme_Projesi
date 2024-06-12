// Description: TutoManager. Manage tutorials at runtime.
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TS.Generics
{
    public class TutoManager : MonoBehaviour
    {
        public int ID;

        [System.Serializable]
        public class StepParams
        {
            public bool         bAutoInit = false;
            public UnityEvent   initStep;
        }

        public List<StepParams> tutoList = new List<StepParams>();

        public bool             bTutoCompleteAutoSave = true;

        public void InitTuto()
        {
            foreach (StepParams step in tutoList)
                if (step.bAutoInit)
                    step.initStep?.Invoke();

            if (bTutoCompleteAutoSave) TutoComplete();
        }


        public void TutoComplete()
        {
            PlayerPrefs.SetString("Tuto_" + ID, "Done");
        } 

    }

}


