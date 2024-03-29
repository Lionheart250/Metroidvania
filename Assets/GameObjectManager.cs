using System.Collections.Generic;
using UnityEngine;

public class GameObjectManager : MonoBehaviour
{
    public List<GameObject> objectsToManage = new List<GameObject>();
    public string doorID;

    private bool objectManagerTriggered; // Use a private variable to track door state

    private void Start()
    {
        SaveData.Instance.LoadEnvironmentData();
        SaveData.Instance.GetDoorState(doorID);
        // Check if the door should be open based on saved state
        if (SaveData.Instance.GetDoorState(doorID))
        {
            objectManagerTriggered = true;
            EnableObjects();
        }
        else
        {
            objectManagerTriggered = false;
            DisableObjects();
        }
    }

    public void EnableObjects()
    {
        foreach (var obj in objectsToManage)
        {
            obj.SetActive(true);
        }
        SaveData.Instance.SetDoorState(doorID, true); // Set door state to open in save data
        SaveData.Instance.SaveEnvironmentData();
        objectManagerTriggered = true; // Set door state to open
    }

    public void DisableObjects()
    {
        foreach (var obj in objectsToManage)
        {
            obj.SetActive(false);
        }
        SaveData.Instance.SetDoorState(doorID, false); // Set door state to closed in save data
        SaveData.Instance.SaveEnvironmentData();
        objectManagerTriggered = false; // Set door state to closed
    }
}
