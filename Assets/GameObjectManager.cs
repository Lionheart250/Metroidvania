using System.Collections.Generic;
using UnityEngine;

public class GameObjectManager : MonoBehaviour
{
    public List<GameObject> objectsToManage = new List<GameObject>();
    void Start()
    {
        DisableObjects();
    }

    public void EnableObjects()
    {
        foreach (var obj in objectsToManage)
        {
            obj.SetActive(true);
        }
    }

    public void DisableObjects()
    {
        foreach (var obj in objectsToManage)
        {
            obj.SetActive(false);
        }
    }
}