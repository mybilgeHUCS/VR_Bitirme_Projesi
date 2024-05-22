using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public float lineLength = 10f; // Length of the line
    public float spawnInterval = 5f; // Time interval for spawning
    public int minBuildings = 0; // Minimum number of buildings to spawn
    public int maxBuildings = 5; // Maximum number of buildings to spawn
    public ObjectPooler objectPooler; // Reference to the ObjectPooler
    [SerializeField ] float spawnerTime = 10f;

    void Start()
    {
        if (objectPooler == null)
        {
            Debug.LogError("ObjectPooler not assigned.");
            return;
        }

        InvokeRepeating("SpawnCityObjects", 0f, spawnInterval);
    }

    void SpawnCityObjects()
    {
        int numberOfObjects = Random.Range(minBuildings, maxBuildings + 1);

        for (int i = 0; i < numberOfObjects; i++)
        {
            // Generate a random position within the line length
            float positionX = Random.Range(transform.position.x - lineLength / 2, transform.position.x + lineLength / 2);
            Vector3 spawnPosition = new Vector3(positionX, transform.position.y, transform.position.z);

            // Spawn the city object from the pool at the calculated position with a rotation of -90 degrees on the x-axis
            objectPooler.SpawnFromPool(spawnPosition, Quaternion.Euler(-90, 0, 0), spawnerTime);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 startPoint = new Vector3(transform.position.x - lineLength / 2, transform.position.y, transform.position.z);
        Vector3 endPoint = new Vector3(transform.position.x + lineLength / 2, transform.position.y, transform.position.z);

        Gizmos.DrawLine(startPoint, endPoint);

        // Draw spawn points (for visualization purposes)
        int numberOfObjects = Random.Range(minBuildings, maxBuildings + 1);
        for (int i = 0; i < numberOfObjects; i++)
        {
            // Generate a random position within the line length
            float positionX = Random.Range(transform.position.x - lineLength / 2, transform.position.x + lineLength / 2);
            Vector3 spawnPosition = new Vector3(positionX, transform.position.y, transform.position.z);
            Gizmos.DrawWireSphere(spawnPosition, 0.1f);
        }
    }
}
