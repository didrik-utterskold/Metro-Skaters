using System.Collections;
using UnityEngine;
using static Collectable;

// Collectable effect for coins: updates score state and plays collection audio.
public class CoinBehaviour : MonoBehaviour, ICollectableEffect
{
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Applies the coin's effect by adding to the player's score and playing a sound effect
    public void ApplyEffect(GameObject player)
    {
        ScoreManager.Instance.AddCoin();
        AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
    }
}