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
        //unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ResetScene();
    }
    //load main menu
    private void ResetScene()
    {
        SceneManager.LoadScene(1);
    }
}
