using UnityEngine.UIElements;
using UnityEngine;

public class UIController : MonoBehaviour
{
    private Label currentScoreLabel;
    private Label highScoreLabel;
    private Label coinsLabel;
    private Label multiplierLabel;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        currentScoreLabel = root.Q<Label>("score-label");
        highScoreLabel = root.Q<Label>("highscore-label");
        coinsLabel = root.Q<Label>("coins-label");
        multiplierLabel = root.Q<Label>("multiplier-label");
        UpdateUI();
    }

    private void UpdateUI()
    {
        currentScoreLabel.text = "Score: " + ScoreManager.Instance.GetCurrentScore();
        highScoreLabel.text = "High Score: " + ScoreManager.Instance.GetHighScore();
        coinsLabel.text = "Coins: " + ScoreManager.Instance.GetCoinCount();
        multiplierLabel.text = "Multiplier: x" + ScoreManager.Instance.GetMultiplier();
    }

    private void Update()
    {
            UpdateUI();
    }
}