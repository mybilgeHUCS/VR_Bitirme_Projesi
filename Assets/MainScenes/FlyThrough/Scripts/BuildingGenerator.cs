using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public List<GameObject> Buildings = new List<GameObject>();
    public float spawnInterval = 1.0f; // Time between spawns
    public float spawnDistance = 10.0f; // Distance at which to spawn buildings from the origin
    public float minAngle = 0f; // Minimum angle for spawning direction
    public float maxAngle = 90f; // Maximum angle for spawning direction
    public float speed = 5.0f; // Speed at which buildings will move

    private float timeSinceLastSpawn;

    void Start()
    {
        timeSinceLastSpawn = spawnInterval; // Initialize timer to spawn immediately on start
    }

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnInterval)
        {
            SpawnBuilding();
            timeSinceLastSpawn = 0;
        }
    }

    void SpawnBuilding()
    {
        if (Buildings.Count == 0)
            return;

        // Choose a random building prefab
        GameObject buildingPrefab = Buildings[Random.Range(0, Buildings.Count)];

        Vector3 spawnPosition = transform.position;
        spawnPosition.x += Random.Range(-spawnDistance, spawnDistance);  

        // Instantiate the building at the calculated position
        GameObject spawnedBuilding = Instantiate(buildingPrefab, spawnPosition, Quaternion.identity);


        // Add a mover script dynamically and set its speed
        Mover mover = spawnedBuilding.AddComponent<Mover>();
        mover.SetSpeed(speed);

        //spawnedBuilding.GetComponent<Transform>().rotation = new Quaternion(-90, 0, 0, 0);
    }
}
