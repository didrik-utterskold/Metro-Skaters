using System.Collections.Generic;
using UnityEngine;

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

        Vector3 offset = newChunk.transform.position - startPoint.position;

        Vector3 fixedPosition = targetStartPosition + offset;

        newChunk.transform.position = fixedPosition;
        
        currentEndPoint = endPoint;

        loadedChunks.Enqueue(newChunk);

        if(loadedChunks.Count > maxLoadedChunks)
        {
            GameObject chunkToRemove = loadedChunks.Dequeue();
            Destroy(chunkToRemove);
        }
    }
}
