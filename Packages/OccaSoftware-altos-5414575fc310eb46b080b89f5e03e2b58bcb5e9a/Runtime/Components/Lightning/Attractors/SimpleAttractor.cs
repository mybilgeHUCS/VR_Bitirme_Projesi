using UnityEngine;

namespace OccaSoftware.Altos.Runtime
{
  [ExecuteAlways]
  [AddComponentMenu("OccaSoftware/Altos/Lightning/Simple Attractor")]
  public class SimpleAttractor : LightningAttractorStrategy
  {
    public override Vector3 GetTarget()
    {
      return transform.position;
    }

    public override float GetPriority(LightningBoltStrategy strategy)
    {
      return Random.Range(0f, 1f);
    }

    public override void Strike(BoltEvent boltEvent)
    {
      LightningEventDispatcher.DispatchStrikeEvent(
        new StrikeEvent()
        {
          position = GetTarget(),
          attractor = this,
          boltEvent = boltEvent
        }
      );
    }
  }
}
