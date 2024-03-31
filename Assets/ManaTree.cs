using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaTree : MonoBehaviour
{
    public float manaGain = 0.1f; // Amount of mana to gain
    public int spawnLimit = 3; // Number of times to spawn coins before destroying the MoneyTree
    public int spawnCount = 0; // Counter for the number of times coins have been spawned
    public List<GameObject> objectsToManage = new List<GameObject>();

    public string SpawnID;
    private ManaOrbsHandler orbsHandler; // Reference to ManaOrbsHandler

    private void Start()
    {
        orbsHandler = FindObjectOfType<ManaOrbsHandler>(); // Find the ManaOrbsHandler in the scene
        LoadState();
        
    }

    public void EnableManaObjects()
    {
        foreach (var obj in objectsToManage)
        {
            obj.SetActive(true);
        }
        SaveState();
    }

    public void DisableManaObjects()
    {
        foreach (var obj in objectsToManage)
        {
            obj.SetActive(false);
        }
        SaveState();
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

        SaveState();

        if (spawnCount >= spawnLimit)
        {

            DisableManaObjects();
        }
    }

    private void SaveState()
    {
        SaveData.Instance.SaveSpawnCount(SpawnID, spawnCount);
        SaveData.Instance.SaveEnvironmentData();
    }

    private void LoadState()
    {
        SaveData.Instance.LoadEnvironmentData();
        spawnCount = SaveData.Instance.GetSpawnCount(SpawnID);
        if (spawnCount >= spawnLimit)
        {
            DisableManaObjects();
        }
        else
        {
            EnableManaObjects();
        }
    }
}
