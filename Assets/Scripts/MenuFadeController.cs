using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFadeController : MonoBehaviour
{
    private FadeUI fadeUI;
    [SerializeField] private float fadeTime;

    // Start is called before the first frame update
    void Start()
    {
        fadeUI = GetComponent<FadeUI>();
        fadeUI.FadeUIOut(fadeTime);
    }

    public void CallFadeAndStartGame(string _sceneToLoad)
    {
        StartCoroutine(FadeAndStartGame(_sceneToLoad));
    }

    public void CallFadeAndClearData()
    {
        StartCoroutine(FadeAndClearData());
    }

    IEnumerator FadeAndStartGame(string _sceneToLoad)
    {
        fadeUI.FadeUIIn(fadeTime);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(_sceneToLoad);
    }

    IEnumerator FadeAndClearData()
    {
        fadeUI.FadeUIIn(fadeTime);
        yield return new WaitForSeconds(fadeTime);

        // Call the method to clear the data
        ClearGameData();

        // Optionally load a specific scene after clearing data
        // SceneManager.LoadScene("MainMenu");
    }

    // Method to clear the game data
    private void ClearGameData()
    {
        // Assuming you have a reference to your SaveData scriptable object
        SaveData.Instance.ClearSavedData();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
