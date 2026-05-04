using UnityEngine.UIElements;
using UnityEngine;

public class UIController : MonoBehaviour
{
    private Label currentScoreLabel;
    private Label highScoreLabel;
    private Label coinsLabel;
    private Label multiplierLabel;
    private Label powerUpTimerLabel;

    private float powerUpTimeRemaining;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        currentScoreLabel = root.Q<Label>("score-label");
        highScoreLabel = root.Q<Label>("highscore-label");
        coinsLabel = root.Q<Label>("coins-label");
        multiplierLabel = root.Q<Label>("multiplier-label");
        powerUpTimerLabel = root.Q<Label>("powerup-timer-label");
        UpdateUI();
    }

    private void UpdateUI()
    {
        currentScoreLabel.text = "Score: " + ScoreManager.Instance.GetCurrentScore();
        highScoreLabel.text = "High Score: " + ScoreManager.Instance.GetHighScore();
        coinsLabel.text = "Coins: " + ScoreManager.Instance.GetCoinCount();
        multiplierLabel.text = "Multiplier: x" + ScoreManager.Instance.GetMultiplier();

        powerUpTimeRemaining = ScoreManager.Instance.GetPowerUpTimeRemaining();

        if (powerUpTimeRemaining > 0f)
        {
            powerUpTimerLabel.text = "Power-Up Time: " + powerUpTimeRemaining.ToString("F1") + "s";
            powerUpTimerLabel.style.display = DisplayStyle.Flex;
        }
        else
        {
            powerUpTimerLabel.style.display = DisplayStyle.None;
        }
    }

    private void Update()
    {
            UpdateUI();
    }
}