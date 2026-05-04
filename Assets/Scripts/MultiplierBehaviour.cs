using UnityEngine;
using static Collectable;

public class MultiplierBehaviour : MonoBehaviour, ICollectableEffect
{
    private AudioSource audioSource;
    private Collectable collectable;
    private float doubleScore = 2.0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        collectable = GetComponent<Collectable>();
    }

    // Applies the multiplier's effect by setting the player's score multiplier and playing a sound effect
    public void ApplyEffect(GameObject player)
    {
        ScoreManager.Instance.SetMultiplier(doubleScore, collectable.GetPowerUpDuration());
        AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
    }
}
