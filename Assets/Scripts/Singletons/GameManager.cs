using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Vector2 platformingRespawnPoint;
    public Vector2 respawnPoint;
    [SerializeField] Vector2 defaultRespawnPoint;
    [SerializeField] Bench bench;

    public GameObject shade;

    [SerializeField] private FadeUI pauseMenu;
    [SerializeField] private float fadeTime;
    public bool gameIsPaused;

    // Added field for storing the previously transitioned from scene
    public string transitionedFromScene;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SaveData.Instance.Initialize();
        //SaveData.Instance.LoadEnvironmentData();


        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Store the previously transitioned from scene
        transitionedFromScene = scene.name;

        SaveScene(scene.name);
        SaveData.Instance.LoadEnvironmentData();

        // Set the player's position to the most recently saved player position
        if (PlayerController.Instance != null && GameManager.Instance.transitionedFromScene != SceneTransition.Instance.TransitionTo)
        {
            PlayerController.Instance.transform.position = SaveData.Instance.playerPosition;
        }

        if (PlayerController.Instance != null && PlayerController.Instance.halfMana)
        {
            SaveData.Instance.LoadShadeData();
            if (SaveData.Instance.sceneWithShade == scene.name || SaveData.Instance.sceneWithShade == "")
            {
                Instantiate(shade, SaveData.Instance.shadePos, SaveData.Instance.shadeRot);
            }
        }

        bench = FindObjectOfType<Bench>();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveData.Instance.SavePlayerData();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !gameIsPaused)
        {
            pauseMenu.FadeUIIn(fadeTime);
            FreezeTime();
            gameIsPaused = true;
        }
    }

    public void UnpauseGame()
    {
        UnFreezeTime();
        gameIsPaused = false;
    }

    public void SaveScene(string sceneName)
    {
        SaveData.Instance.sceneNames.Add(sceneName);
    }

    public void RespawnPlayer()
{
    SaveData.Instance.LoadBench();
    
    // Check if the player has reached a bench and has a valid benchPos
    if (!string.IsNullOrEmpty(SaveData.Instance.benchSceneName) && SaveData.Instance.benchPos != null)
    {
        respawnPoint = SaveData.Instance.benchPos;
    }
    else
    {
        respawnPoint = platformingRespawnPoint; // Use platformingRespawnPoint as the default respawn point
    }

    StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
    PlayerController.Instance.transform.position = respawnPoint;

    PlayerController.Instance.Respawned();
}


    public void FreezeTime()
    {
        Time.timeScale = 0;
    }

    public void UnFreezeTime()
    {
        Time.timeScale = 1;
    }
}
