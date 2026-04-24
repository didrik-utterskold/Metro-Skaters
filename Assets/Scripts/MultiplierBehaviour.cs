using UnityEngine;
using static Collectable;

public class MultiplierBehaviour : MonoBehaviour, ICollectableEffect
{
    private AudioSource audioSource;
    private float doubleScore = 2.0f;
    private float duration = 10.0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Applies the multiplier's effect by setting the player's score multiplier and playing a sound effect
    public void ApplyEffect()
    {
        ScoreManager.Instance.SetMultiplier(doubleScore, duration);
        AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
    }
}
