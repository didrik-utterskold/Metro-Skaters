using UnityEngine;

// Moves a coin toward the player while it is inside the magnet detector.
public class CoinMove : MonoBehaviour
{
    private Transform playerTransform;
    private float moveSpeed = 15f;
    private bool isAttracted = false;

    private void Update()
    {
        if (isAttracted && playerTransform != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                playerTransform.position, 
                moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CoinDetector"))
        {
            // The detector is part of the player hierarchy, so the root is used to find the player transform.
            playerTransform = other.transform.root.GetChild(0);
            isAttracted = true;
        }
    }
}
