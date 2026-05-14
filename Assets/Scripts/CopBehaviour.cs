using UnityEngine;

public class CopBehaviour : MonoBehaviour
{
    [SerializeField] private Transform playerLocation;
    [SerializeField] private float speed;

    void Update()
    {
        Vector3 targetPosition = playerLocation.position;

        transform.LookAt(targetPosition);

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }
}
