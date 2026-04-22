using UnityEngine;

public class CopBehaviour : MonoBehaviour
{
    [SerializeField] private Transform playerLocation;
    [SerializeField] private float speed;

    void Update()
    {
        transform.LookAt(playerLocation);

        transform.position = Vector3.MoveTowards(transform.position, playerLocation.position, speed * Time.deltaTime);
    }
}
