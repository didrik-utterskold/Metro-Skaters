using UnityEngine;

public class Collectable : MonoBehaviour
{
    // Interface for collectable effects, allowing for different types of collectables with unique behaviors
    public interface ICollectableEffect
    {
        void ApplyEffect();
    }
    // Rotates the collectable every frame to make it more visually appealing
    private void Update()
    {
        transform.Rotate(50f * Time.deltaTime * Vector3.up);
    }

    // Detects when the player collides with the collectable, applies the effect, and destroys the collectable
    private void OnTriggerEnter(Collider other)
    {

        ICollectableEffect effect = GetComponent<ICollectableEffect>();

        if (other.CompareTag("Player"))
        {
            effect.ApplyEffect();
            Destroy(gameObject);
        }
    }
}
