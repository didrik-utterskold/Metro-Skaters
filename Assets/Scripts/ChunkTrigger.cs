using UnityEngine;

public class ChunkTrigger : MonoBehaviour
{

    bool hasBeenTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasBeenTriggered = true;
            WorldGeneration.Instance.SpawnChunk();
        }
    }
}
