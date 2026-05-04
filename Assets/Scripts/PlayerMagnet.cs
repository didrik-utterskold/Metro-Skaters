using System.Collections;
using UnityEngine;

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