//Description: DataRef: access scriptable object from any scrit in the scene.
using UnityEngine;

namespace TS.Generics
{
    public class DataRef : MonoBehaviour
    {
        public static DataRef instance = null;            

        [HideInInspector]
        public bool                     SeeInspector;
        [HideInInspector]
        public bool                     moreOptions;
        [HideInInspector]
        public bool                     helpBox = true;

        [Header("Access Main Page build In Scene ID")]
        public GlobalDatas              mainMenuBuildInSceneID;

        [Header("Vehicles Data")]
        public VehicleGlobalData        vehicleGlobalData;
        public DifficultyManagerData    difficultyManagerData;


        [Header("Tracks Data")]
        public TracksData               tracksData;

        [Header("Game Mode Data")]
        public ArcadeModeData           arcadeModeData;
        public ChampionshipModeData     championshipModeData;
        public TimeTrialModeData        timeTrialModeData;

        void Awake()
        {
            #region Create ony one instance of the gameObject in the Hierarchy
            //Check if instance already exists
            if (instance == null)
                //if not, set instance to this
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
            #endregion
        }
    }

}
