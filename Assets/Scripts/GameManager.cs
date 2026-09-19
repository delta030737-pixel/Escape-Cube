using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI; 
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Screens")]
    public GameObject winScreen;
    public GameObject loseScreen;

    [Header("Pause & Options UI")]
    public GameObject pauseScreen;
    public GameObject optionsScreen;
    public Slider sensitivitySlider;          
    public FPS_Cam fpsCam;                     

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
    private bool isPaused;

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
        isPaused = false;

        if (winScreen != null) winScreen.SetActive(false);
        if (loseScreen != null) loseScreen.SetActive(false);
        if (pauseScreen != null) pauseScreen.SetActive(false);
        if (optionsScreen != null) optionsScreen.SetActive(false);
        if (interactTextObj != null) interactTextObj.SetActive(false);

        if (fpsCam != null && sensitivitySlider != null)
        {
            sensitivitySlider.value = fpsCam.mouseSensitivity;
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        }

        UpdateCubeUI();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameEnded)
        {
            if (optionsScreen != null && optionsScreen.activeSelf)
            {
                CloseOptions(); 
            }
            else if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (gameEnded) return;
        isPaused = true;

        if (pauseScreen != null) pauseScreen.SetActive(true);
        if (optionsScreen != null) optionsScreen.SetActive(false);

        ShowInteractUI(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (gameEnded) return;
        isPaused = false;

        if (pauseScreen != null) pauseScreen.SetActive(false);
        if (optionsScreen != null) optionsScreen.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public void OpenOptions()
    {
        if (pauseScreen != null) pauseScreen.SetActive(false);
        if (optionsScreen != null) optionsScreen.SetActive(true);
    }

    public void CloseOptions()
    {
        if (optionsScreen != null) optionsScreen.SetActive(false);
        if (pauseScreen != null) pauseScreen.SetActive(true);
    }

    public void SetSensitivity(float value)
    {
        if (fpsCam != null)
        {
            fpsCam.mouseSensitivity = value;
        }
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
        if (interactTextObj != null && !gameEnded && !isPaused)
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

        if (loseVideoPlayer != null && Video != null)
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