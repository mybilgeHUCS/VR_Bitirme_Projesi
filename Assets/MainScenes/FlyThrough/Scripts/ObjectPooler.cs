using System.Collections;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public void SpawnFromPool(Vector3 position, Quaternion rotation, float delay)
    {
        int childCount = transform.childCount;
        if (childCount == 0)
        {
            Debug.LogWarning("No child objects in the pool to spawn.");
            return;
        }

        int randomIndex = Random.Range(0, childCount);
        Transform chosenChild = transform.GetChild(randomIndex);
        if (chosenChild.gameObject.activeSelf)
        {
            return;
        }
        chosenChild.position = new Vector3(position.x, chosenChild.position.y, position.z);
        chosenChild.rotation = rotation;
        chosenChild.gameObject.SetActive(true);

        StartCoroutine(ReturnToPoolAfterTime(chosenChild.gameObject, delay));
    }

    private IEnumerator ReturnToPoolAfterTime(GameObject objectToReturn, float delay)
    {
        yield return new WaitForSeconds(delay);
        objectToReturn.SetActive(false);
    }
}
