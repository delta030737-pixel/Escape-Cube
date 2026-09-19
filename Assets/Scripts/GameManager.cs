using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Screens")]
    public GameObject winScreen;
    public GameObject loseScreen;

    [Header("Cube Objective")]
    public int totalCubes = 8;
    private int currentCubes = 0;
    public TMP_Text collectText;
    public GameObject collectTextObj;
    public GameObject interactTextObj;

    [Header("Player & Camera Settings")]
    public GameObject player;
    public MonoBehaviour cameraLookScript;

    public GameObject[] enemy;

    [Header("Lose Video Settings")]
    public GameObject Video;
    public VideoPlayer loseVideoPlayer;
    [Tooltip("ใส่จำนวนวินาทีที่ต้องการให้นับถอยหลัง (เช่น 5) / ถ้าใส่ 0 ระบบจะพยายามดึงความยาวคลิปอัตโนมัติ")]
    public float videoCountdown = 0f;

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
        Time.timeScale = 1f;
        gameEnded = false;

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
        if (interactTextObj != null && !gameEnded)
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

        foreach (GameObject enemyobj in enemy)
        {
            Destroy(enemyobj);
        }

        if (loseScreen != null) loseScreen.SetActive(true);

        if (loseVideoPlayer != null)
        {
            loseVideoPlayer.gameObject.SetActive(true);
            loseVideoPlayer.time = 0;
            loseVideoPlayer.Play();

            StartCoroutine(CloseVideoRoutine());
        }

        EndGameCommon();
    }

    private IEnumerator CloseVideoRoutine()
    {
        float waitTime = videoCountdown;

        if (waitTime <= 0f)
        {
            yield return new WaitForSecondsRealtime(0.2f);
            waitTime = (float)loseVideoPlayer.length;

            if (waitTime <= 0f) waitTime = 3f;
        }

        yield return new WaitForSecondsRealtime(waitTime);

        if (loseVideoPlayer != null)
        {
            Video.SetActive(false);
        }
    }

    private void EndGameCommon()
    {
        ShowInteractUI(false);

        if (cameraLookScript != null)
        {
            cameraLookScript.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
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