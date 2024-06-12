// Description: PowerUpsSceneRef: Access from anywhere to UI used by power ups system
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TS.Generics
{
    public class PowerUpsSceneRef : MonoBehaviour
    {
        public static PowerUpsSceneRef          instance = null;
        public bool                             b_InitDone;

        [Header("General")]
        public PowerUpsDatas                    powerUpsDatas;
        //-> Image for P1 | P2 to display the current player power-up (down left corner)
        public List<Image>                      listUIPowerUpIcons = new List<Image>();

        //-> Image (Triangle) for P1 | P2 to display missiles targets
        [Header("06: Missile")]
        public List<UIMissileTargetsByPlayer>   listUIMissileTargetsByPlayer = new List<UIMissileTargetsByPlayer>();    //
        [System.Serializable]
        public class UIMissileTargetsByPlayer
        {
            public List<GameObject> listUIMissileTargets = new List<GameObject>();
        }

        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
        }
    }

}
