using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int CoinCount;

    private float Score;

    private float currentMultiplier;

    public static ScoreManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CoinCount = 0;
        Score = 0f;
        currentMultiplier = 1f;
    }

    private void Update()
    {
        Score = Time.time * currentMultiplier * 100;
    }

    // Getters and Setters
    public int GetCoinCount()
    {
        return CoinCount;
    }

    public void AddCoin()
    {
        CoinCount++;
    }

    public int GetScore()
    {
        return (int)Score;
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
