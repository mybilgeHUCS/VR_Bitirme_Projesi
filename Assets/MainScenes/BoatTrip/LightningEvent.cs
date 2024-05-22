using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OccaSoftware.Altos.Runtime;

public class LightningEvent : MonoBehaviour
{
    [SerializeField] AudioClip lightAudioClip;
     private void OnEnable()
    {
        // Subscribe to the OnBolt event
      //  LightningEventDispatcher.OnBolt += OnBoltEventHandler;
 
        // Subscribe to the OnStrike event
        LightningEventDispatcher.OnStrike += OnStrikeEventHandler;
        Debug.Log("a");
    }
 

    public void OnStrikeEventHandler(StrikeEvent strikeEvent)
    {
        Debug.Log("a");
        // Handle the strike event
        Debug.Log($"Strike occurred at {strikeEvent.position} by {strikeEvent.attractor.gameObject.name}");
        AudioSource.PlayClipAtPoint(lightAudioClip,strikeEvent.position);
    }
}
