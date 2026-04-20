using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int coinCount;

    private float currentScore;

    private float highScore;

    private float currentMultiplier;

    private float elapsedTime;

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
        currentMultiplier = 1f;
        highScore = PlayerPrefs.GetFloat("HighScore", 0f);
    }

    // Calculate score based on elapsed time and multiplier
    private void Update()
    {
        elapsedTime += Time.deltaTime;
        currentScore = (int)(elapsedTime * currentMultiplier * 100);
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

    public float GetCurrentScore()
    {
        return currentScore;
    }

    public float GetHighScore()
    {
        return highScore;
    }

    public float GetMultiplier()
    {
        return currentMultiplier;
    }

    public void SetMultiplier(float multiplier)
    {
        currentMultiplier = multiplier;
    }
}
