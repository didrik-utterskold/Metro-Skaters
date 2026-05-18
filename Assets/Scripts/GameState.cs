using UnityEngine;
using UnityEngine.SceneManagement;

// Watches for losing conditions and restarts the active scene after saving score.
public class GameState : MonoBehaviour
{
    [SerializeField] private Transform playerLocation;
    private bool isCaught = false;
    private float deathThreshold = -10f;

    private void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("Cop"))
        {
            isCaught = true;
        }
    }

    private bool HasFallenOff()
    {
        return playerLocation.position.y < deathThreshold;
    }

    private void Update()
    {
        if (isCaught || HasFallenOff())
        {
            SaveAndReset();
        }
    }

    private void SaveAndReset() 
    {
        ScoreManager.Instance.SaveHighScore();
        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ResetScene();
    }
    // Load main menu
    private void ResetScene()
    {
        SceneManager.LoadScene(0);
    }
}
