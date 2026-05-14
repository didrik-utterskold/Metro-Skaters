using UnityEngine;

// Requests the next world chunk when the player reaches this trigger.
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
