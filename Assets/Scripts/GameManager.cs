using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Screens")]
    public GameObject winScreen;
    public GameObject loseScreen;

    [Header("Cube Objective")]
    public int totalCubes = 8;               // จำนวน Cube ทั้งหมดที่ต้องเก็บ
    private int currentCubes = 0;             // จำนวนที่เก็บได้ตอนนี้
    public TMP_Text collectText;             // UI ข้อความแสดงจำนวน (เช่น 0/8 cubes)
    public GameObject collectTextObj;
    public GameObject interactTextObj;        // UI ข้อความแจ้งเตือนกด E

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
        if (interactTextObj != null) interactTextObj.SetActive(false);

        UpdateCubeUI();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void AddCube()
    {
        if (gameEnded) return;

        currentCubes++;
        UpdateCubeUI();

        if (currentCubes >= totalCubes)
        {
            WinGame();
        }
    }

    public void ShowInteractUI(bool show)
    {
        if (interactTextObj != null)
        {
            interactTextObj.SetActive(show);
        }
    }

    void UpdateCubeUI()
    {
        if (collectTextObj != null) collectTextObj.SetActive(true);
        if (collectText != null) collectText.text = currentCubes + "/" + totalCubes + " cubes";
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
        ShowInteractUI(false);

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