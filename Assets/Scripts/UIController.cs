using UnityEngine.UIElements;
using UnityEngine;

// Connects UI Toolkit labels to live score and power-up data.
public class UIController : MonoBehaviour
{
    private Label currentScoreLabel;
    private Label highScoreLabel;
    private Label coinsLabel;
    private Label multiplierLabel;
    private Label powerUpTimerLabel;

    private float powerUpTimeRemaining;

    private bool initialised = false;

    private void OnEnable()
    {
        // Cache label references by their UXML names once the document is enabled.
        var root = GetComponent<UIDocument>().rootVisualElement;

        currentScoreLabel = root.Q<Label>("score-label");
        highScoreLabel = root.Q<Label>("highscore-label");
        coinsLabel = root.Q<Label>("coins-label");
        multiplierLabel = root.Q<Label>("multiplier-label");
        powerUpTimerLabel = root.Q<Label>("powerup-timer-label");

        initialised = true;
    }

    private void UpdateUI()
    {
        if (!initialised) return;

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