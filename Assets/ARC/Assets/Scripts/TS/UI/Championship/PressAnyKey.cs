// Description: PressAnyKey: Wait until the player press a key to load the nex track in championship mode
using UnityEngine;
using UnityEngine.Events;

namespace TS.Generics
{
    public class PressAnyKey : MonoBehaviour
    {
        private bool bOnce;

        public UnityEvent loadNextTrack;

        void Update()
        {
            if (Input.anyKeyDown && !bOnce && !InfoPlayerTS.instance.b_IsPageCustomPartInProcess)
            {
                bOnce = true;
                loadNextTrack.Invoke();
            }
        }

        //-> Load New Scene
        public void LoadSceneUsingCurrentSelectedTrackName()
        {
            string trackName = GameModeGlobal.instance.currentSelectedTrack;
            LoadScene.instance.LoadSceneWithSceneNameAndSpecificCustomMethodList(trackName);
        }


    }

}
