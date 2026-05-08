using UnityEngine;
using static Collectable;

public class ShoesBehaviour : MonoBehaviour, ICollectableEffect
{
    private AudioSource audioSource;
    [SerializeField] private float jumpBoost = 10f;
    private float duration = 10f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Applies the jump boost effect by increasing the player's jump height and playing a sound effect
    public void ApplyEffect(GameObject player)
    {
        PlayerMovement playerMovement = player.GetComponentInParent<PlayerMovement>();
        ScoreManager.Instance.SetPowerUp();
        playerMovement.SetJumpBoost(jumpBoost, duration);
        AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
    }
}
