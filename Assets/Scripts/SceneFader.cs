using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public float fadeTime;

    [SerializeField] private Image fadeOutUIImage;

    public enum FadeDirection
    {
        In, 
        Out
    }

    // Start is called before the first frame update
    void Start()
{
    fadeOutUIImage = GetComponent<Image>();

    if (fadeOutUIImage == null)
    {
        Debug.LogError("Image component not found on SceneFader object.");
    }
    else
    {
        Debug.Log("Image component found and assigned successfully.");
    }
}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CallFadeAndLoadScene(string _sceneToLoad)
    {
        StartCoroutine(FadeAndLoadScene(FadeDirection.In, _sceneToLoad));
    }

    public IEnumerator Fade(FadeDirection _fadeDirection)
    {
        float _alpha = _fadeDirection == FadeDirection.Out ? 1 : 0;
        float _fadeEndValue = _fadeDirection == FadeDirection.Out ? 0 : 1;

        if(_fadeDirection == FadeDirection.Out)
        {
            while(_alpha >= _fadeEndValue)
            {
                SetColorImage(ref _alpha, _fadeDirection);

                yield return null;
            }

            fadeOutUIImage.enabled = false;
        }
        else
        {
            fadeOutUIImage.enabled = true;

            while (_alpha <= _fadeEndValue)
            {
                SetColorImage(ref _alpha, _fadeDirection);

                yield return null;
            }
        }
    }

    public IEnumerator FadeAndLoadScene(FadeDirection _fadeDirection, string _sceneToLoad)
    {   
        

        fadeOutUIImage.enabled = true;

        yield return Fade(_fadeDirection);

        SceneManager.LoadScene(_sceneToLoad);
    }

   void SetColorImage(ref float _alpha, FadeDirection _fadeDirection)
{
    if (fadeOutUIImage != null)
    {
        fadeOutUIImage.color = new Color(fadeOutUIImage.color.r, fadeOutUIImage.color.g, fadeOutUIImage.color.b, _alpha);
        _alpha += 0.02f * (_fadeDirection == FadeDirection.Out ? -1 : 1);
    }
    else
    {
        Debug.LogError("fadeImageUI is not assigned. Make sure it is assigned in the Unity Editor.");
    }
}

}
