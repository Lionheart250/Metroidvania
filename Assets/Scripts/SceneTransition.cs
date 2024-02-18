using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string transitionTo;

    [SerializeField] private Transform startPoint;

    [SerializeField] private Vector2 exitDirection;

    [SerializeField] private float exitTime;

    // Start is called before the first frame update
    private void Awake()
{
    // Check if GameManager.Instance is null
    if (GameManager.Instance == null)
    {
        Debug.LogError("GameManager.Instance is null!");
        return; // Exit the Awake method to prevent further issues
    }

    // Check if UIManager.Instance is null
    if (UIManager.Instance == null)
    {
        Debug.LogError("UIManager.Instance is null!");
        return; // Exit the Awake method to prevent further issues
    }

    // Check the value of GameManager.Instance.transitionedFromScene
    // Debug.Log("GameManager.Instance.transitionedFromScene: " + GameManager.Instance.transitionedFromScene);

    // Rest of your Awake method
    if (GameManager.Instance.transitionedFromScene == transitionTo)
    {
        PlayerController.Instance.transform.position = startPoint.position;
        PlayerController3D.Instance.transform.position = startPoint.position;

        StartCoroutine(PlayerController.Instance.WalkIntoNewScene(exitDirection, exitTime));
        StartCoroutine(PlayerController3D.Instance.WalkIntoNewScene(exitDirection, exitTime));
    }
    StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
}



    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            CheckShadeData();

            GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;

            PlayerController.Instance.pState.cutscene = true;

            StartCoroutine(UIManager.Instance.sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, transitionTo));
        }
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.CompareTag("Player3D"))
        {
            CheckShadeData();

            GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;


            StartCoroutine(UIManager.Instance.sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, transitionTo));
        }
    }
    void CheckShadeData()
{
    GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

    if (enemyObjects != null)
    {
        for (int i = 0; i < enemyObjects.Length; i++)
        {
            if (enemyObjects[i] != null && enemyObjects[i].GetComponent<Shade>() != null)
            {
                SaveData.Instance.SaveShadeData();
            }
        }
    }
}
}