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

        Transform startPoint = newChunk.transform.Find("BasePlate/StartPoint");
        Transform endPoint = newChunk.transform.Find("BasePlate/EndPoint");

        Vector3 offset = newChunk.transform.position - startPoint.position;

        newChunk.transform.position = targetStartPosition + offset;

        currentEndPoint = endPoint;

    }
}
