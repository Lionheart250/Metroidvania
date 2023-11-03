using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MapManager : MonoBehaviour
{
    [SerializeField] GameObject[] maps;

    Bench bench;
    // Start is called before the first frame update
    private void OnEnable()
    {
        bench = FindObjectOfType<Bench>();
        if (bench != null)
        {
            if (bench.interacted)
            {
                UpdateMap();
            }
        }
    }

    void UpdateMap()
    {
        Debug.Log("updated");

        // Activate all maps
        foreach (var map in maps)
        {
            map.SetActive(true);
        }
    }
}
