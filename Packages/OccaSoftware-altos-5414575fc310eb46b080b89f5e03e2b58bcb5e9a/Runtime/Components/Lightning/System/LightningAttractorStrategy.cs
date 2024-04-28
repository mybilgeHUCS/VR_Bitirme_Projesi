using UnityEngine;

namespace OccaSoftware.Altos.Runtime
{
  public abstract class LightningAttractorStrategy : MonoBehaviour
  {
    private void OnEnable()
    {
      LightningAttractorSystem.Add(this);
    }

    private void OnDisable()
    {
      LightningAttractorSystem.Remove(this);
    }

    abstract public Vector3 GetTarget();

    abstract public float GetPriority(LightningBoltStrategy bolt);

    abstract public void Strike(BoltEvent bolt);
  }
}
