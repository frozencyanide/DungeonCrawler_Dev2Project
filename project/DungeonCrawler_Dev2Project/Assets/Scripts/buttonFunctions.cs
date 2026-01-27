using System.Data;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void Resume()
    {
        GameManager.instance.UnpauseGame();
    }

    public void Respawn()
    {
        GameManager.instance.pController.RespawnPlayer();
        GameManager.instance.UnpauseGame();
        GameManager.instance.pController.UpdatePlayerUI();
    }


    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.UnpauseGame();
    }

    public void Quit()
    {
        GameManager.instance.activeEnemies.Clear();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();

#endif
    }
}
