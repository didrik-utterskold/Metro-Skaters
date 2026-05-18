using TMPro;
using UnityEngine;

public class HardModeButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI myText;

    // Ensure hard mode is off by default
    private void Start()
    {
        GameSettings.HardMode = false;
    }

    public void StartHardMode()
    {
        GameSettings.HardMode = true;
        ShowMyText();
    }
    
    private void ShowMyText()
    {
        myText.gameObject.SetActive(true);
    }
}