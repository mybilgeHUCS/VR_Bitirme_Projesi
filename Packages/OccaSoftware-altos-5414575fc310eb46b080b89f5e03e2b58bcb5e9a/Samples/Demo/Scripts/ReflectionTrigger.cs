using UnityEngine;

using OccaSoftware.Altos.Runtime;

namespace OccaSoftware.Altos.Demo
{
    public class ReflectionTrigger : MonoBehaviour, ITriggerReflectionBaking
    {
        private ReflectionBaker baker;

        private void OnTriggerEnter(Collider other)
        {
            Setup();
            Add();
        }

        private void OnTriggerExit(Collider other)
        {
            Remove();
        }

        private void OnDisable() => Remove();

        private void Setup()
        {
            baker = FindObjectOfType<ReflectionBaker>();
        }

        private void Add()
        {
            if (baker)
            {
                baker.ReflectionTriggers.Add(this);
                Debug.Log($"Added {this} to {baker}.");
            }
        }

        private void Remove()
        {
            if (baker)
            {
                baker.ReflectionTriggers.Remove(this);
                Debug.Log($"Removed {this} from {baker}.");
            }
        }
    }
}
