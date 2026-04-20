using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int coinCount;

    private float currentScore;

    private float highScore;

    private float currentMultiplier;

    private float elapsedTime;

    public static ScoreManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        coinCount = 0;
        currentScore = 0f;
        currentMultiplier = 1f;
        highScore = PlayerPrefs.GetFloat("HighScore", 0f);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        currentScore = elapsedTime * currentMultiplier * 100;

        if (currentScore > highScore)
        {
            highScore = currentScore;
        }
    }

    public void SaveHighScore()
    {
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

    public float GetScore()
    {
        return currentScore;
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
