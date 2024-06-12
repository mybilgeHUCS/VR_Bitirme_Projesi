// Description: AudioAssistant: Used by the SceneInitManager in the Main Menu to start the Main Menu music
using UnityEngine;

namespace TS.Generics
{
    public class AudioAssistant : MonoBehaviour
    {
        public bool PlayMenuMusic()
        {
            //Debug.Log("PlayMusic");
            MusicManager.instance.MCrossFade(1,MusicList.instance.ListAudioClip[1]);
            return true;
        }
    }
}
