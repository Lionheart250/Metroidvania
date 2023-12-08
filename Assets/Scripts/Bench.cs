using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class Bench : MonoBehaviour
{
    public bool interacted;
    public Color interactableColor = Color.cyan; 
    private Color originalColor;
    private Renderer benchRenderer;

    // Start is called before the first frame update
    void Start()
    {
        benchRenderer = GetComponent<Renderer>();
        originalColor = benchRenderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay2D(Collider2D _collision)
    {   if (_collision.CompareTag("Player"))
    {
        benchRenderer.material.color = interactableColor;
    }

        if(_collision.CompareTag("Player") && Input.GetButtonDown("Interact")|| (Gamepad.current?.triangleButton.isPressed == true))
        {
            interacted = true;
            PlayerController.Instance.Health = PlayerController.Instance.maxHealth;
            //
            PlayerController.Instance.Mana = 1;// will move this to a seperate game object specifically for refilling the mana 
            //
            SaveData.Instance.benchSceneName = SceneManager.GetActiveScene().name;
            SaveData.Instance.benchPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            SaveData.Instance.SaveBench();
            SaveData.Instance.SavePlayerData();
            Debug.Log("benched");
            benchRenderer.material.color = originalColor;

        }
    }
    private void OnTriggerExit2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player"))
        {
            interacted = false;
            benchRenderer.material.color = originalColor;
        }
    }
}
