using UnityEngine;
using static Collectable;

public class MagnetBehaviour : MonoBehaviour, ICollectableEffect
{
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void ApplyEffect(GameObject player)
    {

        PlayerMagnet playerMagnet = player.GetComponent<PlayerMagnet>();
        
        playerMagnet.ActivateMagnet();

        AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);

    }
}
