using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("Cop"))
        {
            Caught();
        }
    }

    private void Caught() 
    {
        ScoreManager.Instance.SaveHighScore();
        ResetScene();
    }

    private void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
