using UnityEngine;

public class Collectable : MonoBehaviour
{
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Rotates the collectable every frame to make it more visually appealing
    private void Update()
    {
        transform.Rotate(50f * Time.deltaTime * Vector3.up);
    }

     private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ScoreManager.Instance.AddCoin();
            Collect();
        }
    }

    private void Collect()
    {
        AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
        Destroy(gameObject);
    }
}
