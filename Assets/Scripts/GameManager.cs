using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Screens")]
    public GameObject winScreen;
    public GameObject loseScreen;

    [Header("Player")]
    public GameObject player; 

    private bool gameEnded;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (winScreen != null) winScreen.SetActive(false);
        if (loseScreen != null) loseScreen.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void WinGame()
    {
        if (gameEnded) return;
        gameEnded = true;

        if (winScreen != null) winScreen.SetActive(true);
        EndGameCommon();
    }

    public void LoseGame()
    {
        if (gameEnded) return;
        gameEnded = true;

        if (loseScreen != null) loseScreen.SetActive(true);
        EndGameCommon();
    }

    private void EndGameCommon()
    {
        if (player != null)
        {
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
            foreach (var s in scripts)
                s.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}