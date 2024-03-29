using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaTree : MonoBehaviour
{
    public float manaGain = 0.1f; // Amount of mana to gain
    public int spawnLimit = 3; // Number of times to add mana before destroying the ManaTree
    private int spawnCount = 0; // Counter for the number of times mana has been added
    public List<GameObject> objectsToManage = new List<GameObject>();
    public string doorID;

    private bool doorOpen; // Use a private variable to track door state

    private ManaOrbsHandler orbsHandler; // Reference to ManaOrbsHandler

    private void Start()
    {
        orbsHandler = FindObjectOfType<ManaOrbsHandler>(); // Find the ManaOrbsHandler in the scene

        SaveData.Instance.LoadEnvironmentData();
        SaveData.Instance.GetDoorState(doorID);
        // Check if the door should be open based on saved state
        if (SaveData.Instance.GetDoorState(doorID))
        {
            doorOpen = true;
            DisableManaObjects();
        }
        else
        {
            doorOpen = false;
            EnableManaObjects();
        }
    }

    public void EnableManaObjects()
    {
        foreach (var obj in objectsToManage)
        {
            obj.SetActive(true);
        }
        SaveData.Instance.SetDoorState(doorID, false); // Set door state to open in save data
        SaveData.Instance.SaveEnvironmentData();
        doorOpen = true; // Set door state to open
    }

    public void DisableManaObjects()
    {
        foreach (var obj in objectsToManage)
        {
            obj.SetActive(false);
        }
        SaveData.Instance.SetDoorState(doorID, true); // Set door state to closed in save data
        SaveData.Instance.SaveEnvironmentData();
        doorOpen = false; // Set door state to closed
    }

    public void AddMana()
    {
        if (PlayerController.Instance.Mana < 1f)
        {
            PlayerController.Instance.Mana += manaGain;
        }

        if (PlayerController.Instance.Mana >= 1f || (PlayerController.Instance.halfMana && PlayerController.Instance.Mana >= 0.5f))
        {
            if (orbsHandler != null)
            {
                orbsHandler.UpdateMana(manaGain * 3);
            }
            else
            {
                Debug.LogWarning("ManaOrbsHandler not found.");
            }
        }

        spawnCount++;

        if (spawnCount >= spawnLimit)
        {
            Destroy(gameObject);
            doorOpen = true;
            DisableManaObjects();
        }
    }
}
