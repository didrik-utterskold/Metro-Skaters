using System.Collections;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int coinCount;

    private float currentScore;

    private float highScore;

    private float currentMultiplier;

    private float defaultMultiplier = 1f;

    private Coroutine multiplierCoroutine;

    public static ScoreManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Reset score, coins, and load high score from PlayerPrefs
    private void Start()
    {
        coinCount = 0;
        currentScore = 0f;
        currentMultiplier = defaultMultiplier;
        highScore = PlayerPrefs.GetFloat("HighScore", 0f);
    }

    // Calculate score based on elapsed time and multiplier
    private void Update()
    {
        currentScore += Time.deltaTime * currentMultiplier * 100f;
    }

    public void SaveHighScore()
    {
        if (highScore > currentScore)
        {
            return;
        }

        highScore = currentScore;
        PlayerPrefs.SetFloat("HighScore", highScore);
        PlayerPrefs.Save();
    }

    // Getters and Setters
    public int GetCoinCount()
    {
        return coinCount;
    }

    public void AddCoin()
    {
        coinCount++;
    }

    public int GetCurrentScore()
    {
        return (int)currentScore;
    }

    public int GetHighScore()
    {
        return (int)highScore;
    }

    public float GetMultiplier()
    {
        return currentMultiplier;
    }

    public void SetMultiplier(float multiplier, float duration)
    {
        if (multiplierCoroutine != null)
        {
            StopCoroutine(multiplierCoroutine);
        }

        currentMultiplier = multiplier;
        multiplierCoroutine = StartCoroutine(MultiplierRoutineDuration(duration));
    }

    private IEnumerator MultiplierRoutineDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        currentMultiplier = defaultMultiplier;
    }
}
