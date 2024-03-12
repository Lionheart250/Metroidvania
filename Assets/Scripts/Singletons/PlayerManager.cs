using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public GameObject player;
    public GameObject player3D;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Animal_Crossing_World")
        {
            if (player3D != null)
            {
                player3D.SetActive(true);
            }
            if (player != null)
            {
                player.SetActive(false);
            }
        }
        else
        {
            if (player != null)
            {
                player.SetActive(true);
            }
            if (player3D != null)
            {
                player3D.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        Debug.Log("PlayerManager is being destroyed.");
        if (player == null)
        {
            Debug.Log("Player GameObject is destroyed.");
        }
        if (player3D == null)
        {
            Debug.Log("Player3D GameObject is destroyed.");
        }
    }
}
