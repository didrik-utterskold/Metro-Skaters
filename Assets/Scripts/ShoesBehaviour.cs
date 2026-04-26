using UnityEngine;
using static Collectable;

public class ShoesBehaviour : MonoBehaviour, ICollectableEffect
{
    private AudioSource audioSource;
    private float jumpBoost = 10f;
    private float duration = 10.0f;
    [SerializeField] private PlayerMovement playerMovement;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Applies the jump boost effect by increasing the player's jump height and playing a sound effect
    public void ApplyEffect()
    {
        playerMovement.SetJumpBoost(jumpBoost, duration);
        AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
    }
}
