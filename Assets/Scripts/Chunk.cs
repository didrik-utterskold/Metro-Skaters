using UnityEngine;

// Stores the connection points used to line up one generated chunk with the next.
public class Chunk : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    public Transform StartPoint => startPoint;
    public Transform EndPoint => endPoint;
}
