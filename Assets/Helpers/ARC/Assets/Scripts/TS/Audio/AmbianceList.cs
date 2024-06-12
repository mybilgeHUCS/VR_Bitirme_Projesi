//Description: AmbianceList. Work in association with AmbianceManager.
//It is a list of AudioClip
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class AmbianceList : MonoBehaviour
    {
        public static AmbianceList  instance = null;

        public bool                 SeeInspector;
        public bool                 helpBox;
        public bool                 moreOptions;

        public List<AudioClip>      ListAudioClip = new List<AudioClip>();

        void Awake()
        {
            #region Create only one instance of the gameObject in the Hierarchy
            //Check if instance already exists
            if (instance == null)
                //if not, set instance to this
                instance = this;

            #endregion
        }
    }
}