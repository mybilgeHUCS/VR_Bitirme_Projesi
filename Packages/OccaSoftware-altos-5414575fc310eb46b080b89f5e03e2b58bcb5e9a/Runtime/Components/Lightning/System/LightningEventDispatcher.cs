using System;

using UnityEngine;
using UnityEngine.Events;

namespace OccaSoftware.Altos.Runtime
{
  public class BoltEvent
  {
    public Vector3 position;
    public LightningBoltStrategy bolt;
  }

  public class StrikeEvent
  {
    public Vector3 position;
    public LightningAttractorStrategy attractor;
    public BoltEvent boltEvent;
  }

  [AddComponentMenu("OccaSoftware/Altos/Lightning/Event Dispatcher")]
  public class LightningEventDispatcher : MonoBehaviour
  {
    public static Action<BoltEvent> OnBolt;
    public static Action<StrikeEvent> OnStrike;

    public UnityEvent<BoltEvent> OnBoltUnityEvent;
    public UnityEvent<StrikeEvent> OnStrikeUnityEvent;

    public static void DispatchBoltEvent(BoltEvent boltEvent)
    {
      OnBolt?.Invoke(boltEvent);
    }

    public static void DispatchStrikeEvent(StrikeEvent strikeEvent)
    {
      OnStrike?.Invoke(strikeEvent);
    }

    private void OnEnable()
    {
      OnBolt += OnBoltEventHandler;
      OnStrike += OnStrikeEventHandler;
    }

    private void OnDisable()
    {
      OnBolt -= OnBoltEventHandler;
      OnStrike -= OnStrikeEventHandler;
    }

    private void OnBoltEventHandler(BoltEvent boltEvent)
    {
      OnBoltUnityEvent?.Invoke(boltEvent);
    }

    private void OnStrikeEventHandler(StrikeEvent strikeEvent)
    {
      OnStrikeUnityEvent?.Invoke(strikeEvent);
    }
  }
}
