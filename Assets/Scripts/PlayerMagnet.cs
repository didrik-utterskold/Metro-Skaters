using System.Collections;
using UnityEngine;

// Controls the player-side detector that attracts coins during the magnet power-up.
public class PlayerMagnet : MonoBehaviour
{
    [SerializeField] private GameObject coinDetector;
    private float magnetDuration = 10f;

    private Coroutine magnetCoroutine;
    void Start()
    {
        coinDetector.SetActive(false);
    }

    public void ActivateMagnet()
    {
        if (magnetCoroutine != null)
        {
            // Restarting the coroutine refreshes the duration when another magnet is collected.
            StopCoroutine(magnetCoroutine);
        }
        magnetCoroutine = StartCoroutine(MagnetTimer());
    }

    private IEnumerator MagnetTimer()
    {
        coinDetector.SetActive(true);
        yield return new WaitForSeconds(magnetDuration);
        coinDetector.SetActive(false);
    }
}   