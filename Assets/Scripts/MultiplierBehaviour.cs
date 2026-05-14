using UnityEngine;
using static Collectable;

// Collectable effect that doubles score gain for the power-up duration.
public class MultiplierBehaviour : MonoBehaviour, ICollectableEffect
{
    private AudioSource audioSource;
    private float doubleScore = 2.0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Applies the multiplier's effect by setting the player's score multiplier and playing a sound effect
    public void ApplyEffect(GameObject player)
    {
        ScoreManager.Instance.SetPowerUp();
        ScoreManager.Instance.SetMultiplier(doubleScore);

        AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
    }
}
