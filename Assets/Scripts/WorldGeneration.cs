using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    private List<GameObject> chunks = new();

    [SerializeField] private Transform currentEndPoint;

    private void Awake()
    {
        chunks.AddRange(Resources.LoadAll<GameObject>("Chunks/"));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SpawnChunk(currentEndPoint.position);
        }
    }

    public void SpawnChunk(Vector3 targetStartPosition)
    {
        GameObject selectedRandomChunk = chunks[Random.Range(0, chunks.Count)];
       
        GameObject newChunk = Instantiate(selectedRandomChunk);

        Chunk chunkData = newChunk.GetComponent<Chunk>();

        Transform startPoint = chunkData.StartPoint;
        Transform endPoint = chunkData.EndPoint;

        Vector3 offset = newChunk.transform.position - startPoint.position;

        Vector3 fixedPosition = targetStartPosition + offset;

        newChunk.transform.position = fixedPosition;
        
        currentEndPoint = endPoint;
    }
}
