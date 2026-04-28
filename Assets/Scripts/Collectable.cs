using UnityEngine;

public class Collectable : MonoBehaviour
{
    // Interface that defines the method for applying the collectable's effect to the player
    public interface ICollectableEffect
    {
        void ApplyEffect(GameObject player);
    }
    // Rotates the collectable every frame to make it more visually appealing
    private void Update()
    {
        transform.Rotate(50f * Time.deltaTime * Vector3.up);
    }

    // Detects when the player collides with the collectable, applies the effect, and destroys the collectable
    private void OnTriggerEnter(Collider player)
    {

        ICollectableEffect effect = GetComponent<ICollectableEffect>();

        if (player.CompareTag("Player"))
        {
            effect.ApplyEffect(player.gameObject);
            Destroy(gameObject);
        }
    }
}
