using System.Collections.Generic;

using UnityEngine;

namespace OccaSoftware.Altos.Runtime
{
  [AddComponentMenu("OccaSoftware/Altos/Lightning/Bolt Controller")]
  [ExecuteAlways]
  public class LightningBoltController : MonoBehaviour
  {
    public float frequency = 1;

    private float timer;

    /// <summary>
    /// The controller will roll a random strategy for each spawn attempt.
    /// </summary>
    public List<LightningBoltStrategy> strategies = new List<LightningBoltStrategy>();

    public static LightningBoltController instance;

    public bool executeInEditMode = false;

    private void OnEnable()
    {
      instance = this;
    }

    private void OnDisable()
    {
      if (instance == this)
      {
        instance = null;
      }
    }

    private void Update()
    {
      if (!executeInEditMode && !Application.isPlaying)
        return;

      timer -= Time.deltaTime;
      if (timer < 0)
      {
        timer += frequency; // Add randomness
        if (strategies.Count > 0)
        {
          strategies[Random.Range(0, strategies.Count)].GenerateLightning();
        }
      }
    }

    public void GenerateLightning(LightningBoltStrategy boltStrategy)
    {
      boltStrategy.GenerateLightning();
    }
  }
}
