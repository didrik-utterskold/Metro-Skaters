using UnityEngine;

// Simple pursuer that constantly faces and moves toward the player.
public class CopBehaviour : MonoBehaviour
{
    [SerializeField] private Transform playerLocation;
    [SerializeField] private float speed;

    private void Start()
    {
        if (GameSettings.HardMode)
        {
            speed *= 1.2f;
        }
    }

    void Update()
    {
        Vector3 targetPosition = playerLocation.position;

        transform.LookAt(targetPosition);

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    
}
