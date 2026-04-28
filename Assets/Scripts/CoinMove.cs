using UnityEngine;

public class CoinMove : MonoBehaviour
{
    private Transform playerTransform;
    private float moveSpeed = 12f;
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
            playerTransform = other.transform.root;
            isAttracted = true;
        }
    }
}
