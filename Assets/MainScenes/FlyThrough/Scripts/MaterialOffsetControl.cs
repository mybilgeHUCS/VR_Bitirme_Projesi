using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialOffsetControl : MonoBehaviour
{
    public Vector2 normalMapOffsetSpeed = new Vector2(0.1f, 0.1f); // Speed of offset change

    private Renderer renderer;
    private Material material;

    void Start()
    {
        // Get the renderer component
        renderer = GetComponent<Renderer>();

        // Get the material of the renderer
        if (renderer != null)
        {
            material = renderer.material;
        }
        else
        {
            Debug.LogError("No Renderer found on the object.");
        }

        // Check if the material has a normal map
        if (material != null && !material.HasProperty("_BumpMap"))
        {
            Debug.LogError("Material does not contain a normal map.");
        }
    }

    void Update()
    {
        if (material != null && material.HasProperty("_BumpMap"))
        {
            // Adjust the texture offset
            Vector2 currentOffset = material.GetTextureOffset("_BumpMap");
            Vector2 newOffset = currentOffset + normalMapOffsetSpeed * Time.deltaTime;
            material.SetTextureOffset("_BumpMap", newOffset);
        }
    }
}
