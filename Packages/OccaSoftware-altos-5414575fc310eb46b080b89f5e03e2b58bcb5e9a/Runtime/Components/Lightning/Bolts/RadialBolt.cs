using System.Collections;

using UnityEngine;
using UnityEngine.VFX;

namespace OccaSoftware.Altos.Runtime
{
  [ExecuteAlways]
  [AddComponentMenu("OccaSoftware/Altos/Lightning/Radial Bolt")]
  public class RadialBolt : LightningBoltStrategy
  {
    public float spawnRadius = 5000f;

    public override void GenerateLightning()
    {
      Vector3 position = SpawnLightning();
      Instantiate(gameObject, position, Quaternion.identity);
    }

    private Vector3 SpawnLightning()
    { // Set position
      Vector2 xz = Random.insideUnitCircle * spawnRadius;
      float x = xz.x;
      float y = Random.Range(
        AltosSkyDirector.Instance.cloudDefinition.GetCloudFloor(),
        AltosSkyDirector.Instance.cloudDefinition.GetCloudCeiling()
      );
      float z = xz.y;

      return new Vector3(x, y, z);
    }

    private void OnEnable()
    {
      if (LightningBoltController.instance != null)
      {
        gameObject.transform.SetParent(LightningBoltController.instance.transform);
      }

      LightningEventDispatcher.DispatchBoltEvent(
        new BoltEvent() { bolt = this, position = transform.position }
      );

      age = 0;
    }

    float age;

    private void Update()
    {
      age += Time.deltaTime;
      if (age > 5f)
      {
        Dispose();
      }
    }

    private void Dispose()
    {
      if (Application.isPlaying)
      {
        Destroy(gameObject);
      }
      else
      {
        DestroyImmediate(gameObject);
      }
    }
  }
}
