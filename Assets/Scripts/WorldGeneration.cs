using System.Collections.Generic;
using UnityEngine;

// Builds the endless path by spawning chunk prefabs and removing old chunks.
public class WorldGeneration : MonoBehaviour
{
    [SerializeField] private int maxLoadedChunks = 3;
    [SerializeField] private Transform currentEndPoint;
    [SerializeField] private GameObject firstChunk;

    private List<GameObject> chunks = new();
    private Queue<GameObject> loadedChunks = new();

    public static WorldGeneration Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Chunk prefabs are loaded from Resources so new level pieces can be added without code changes.
        chunks.AddRange(Resources.LoadAll<GameObject>("Chunks/"));
    }

    private void Start()
    {
        loadedChunks.Enqueue(firstChunk);
    }

    public void SpawnChunk()
    {
        SpawnChunk(currentEndPoint.position);
    }

    private void SpawnChunk(Vector3 targetStartPosition)
    {
        GameObject selectedRandomChunk = chunks[Random.Range(0, chunks.Count)];
       
        GameObject newChunk = Instantiate(selectedRandomChunk);

        Chunk chunkData = newChunk.GetComponent<Chunk>();

        Transform startPoint = chunkData.StartPoint;
        Transform endPoint = chunkData.EndPoint;

        // Move the chunk so its start anchor lands exactly on the requested connection point.
        Vector3 offset = newChunk.transform.position - startPoint.position;

        Vector3 fixedPosition = targetStartPosition + offset;

        newChunk.transform.position = fixedPosition;
        
        currentEndPoint = endPoint;

        loadedChunks.Enqueue(newChunk);

        if(loadedChunks.Count > maxLoadedChunks)
        {
            // Remove the oldest chunk to keep the scene from growing forever.
            GameObject chunkToRemove = loadedChunks.Dequeue();
            Destroy(chunkToRemove);
        }
    }
}
