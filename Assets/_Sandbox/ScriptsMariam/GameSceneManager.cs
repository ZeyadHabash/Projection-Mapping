using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    // Singleton 
    public static GameSceneManager Instance;

    // Enum 
    public enum Scene
    {
        CalibrationScene = 0,
        CollectionScene = 1,
        ProtectionScene = 2,
        // Add more scenes 
    }

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {

            Destroy(gameObject);
        }
    }

    public void LoadScene(Scene scene)
    {
        SceneManager.LoadScene((int)scene);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
