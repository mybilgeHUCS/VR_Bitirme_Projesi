using System.Collections.Generic;

using UnityEngine;

namespace OccaSoftware.Altos.Runtime
{
  public static class LightningAttractorSystem
  {
    private static List<LightningAttractorStrategy> Attractors =
      new List<LightningAttractorStrategy>();

    public static void Add(LightningAttractorStrategy attractor)
    {
      Attractors.Add(attractor);
    }

    public static void Remove(LightningAttractorStrategy attractor)
    {
      Attractors.Remove(attractor);
    }

    public static List<LightningAttractorStrategy> GetAll()
    {
      return Attractors;
    }

    public static LightningAttractorStrategy GetHighestPriorityAttractor(
      LightningBoltStrategy strategy
    )
    {
      LightningAttractorStrategy highestPriorityAttractor = null;
      float highestPriority = -1;

      foreach (var attractor in Attractors)
      {
        float currentPriority = attractor.GetPriority(strategy);

        if (currentPriority > highestPriority)
        {
          highestPriority = currentPriority;
          highestPriorityAttractor = attractor;
        }
      }

      return highestPriorityAttractor;
    }

    public static LightningAttractorStrategy GetLowestPriorityAttractor(
      LightningBoltStrategy strategy
    )
    {
      LightningAttractorStrategy lowestPriorityAttractor = null;
      float lowestPriority = Mathf.Infinity;

      foreach (var attractor in Attractors)
      {
        float currentPriority = attractor.GetPriority(strategy);

        if (currentPriority < lowestPriority)
        {
          lowestPriority = currentPriority;
          lowestPriorityAttractor = attractor;
        }
      }

      return lowestPriorityAttractor;
    }
  }
}
